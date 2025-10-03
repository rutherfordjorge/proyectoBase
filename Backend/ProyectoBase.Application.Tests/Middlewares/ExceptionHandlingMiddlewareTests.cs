using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ProyectoBase.Api.Api.Errors;
using ProyectoBase.Api.Api.Middlewares;
using ProyectoBase.Api.Domain;
using DomainNotFoundException = ProyectoBase.Api.Domain.Exceptions.NotFoundException;
using DomainValidationException = ProyectoBase.Api.Domain.Exceptions.ValidationException;
using Xunit;

namespace ProyectoBase.Api.Application.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Should_convert_fluent_validation_exception_into_bad_request_response()
    {
        static Task Next(HttpContext _) => throw new ValidationException(new[]
        {
            new ValidationFailure("Name", "The name field is required."),
            new ValidationFailure("Name", "The name field is required."),
            new ValidationFailure("Price", "The price must be greater than zero."),
            new ValidationFailure("Ignored", string.Empty),
        });

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        var error = response.GetProperty("error");
        Assert.Equal(ApiErrorCodes.RequestValidation, error.GetProperty("code").GetString());
        Assert.Equal("Solicitud inválida", error.GetProperty("message").GetString());

        var details = response.GetProperty("details").EnumerateArray()
            .Select(element => new ErrorDetail(
                element.GetProperty("code").GetString(),
                element.GetProperty("message").GetString()))
            .ToArray();

        Assert.Equal(2, details.Length);
        Assert.All(details, detail => Assert.Equal(ApiErrorCodes.RequestValidation, detail.Code));
        Assert.Contains("The name field is required.", details.Select(detail => detail.Message));
        Assert.Contains("The price must be greater than zero.", details.Select(detail => detail.Message));
    }

    [Fact]
    public async Task Should_convert_domain_validation_exception_into_bad_request_response()
    {
        static Task Next(HttpContext _) => throw new DomainValidationException(DomainErrors.Product.IdRequired);

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        var error = response.GetProperty("error");
        Assert.Equal(DomainErrors.Product.IdRequired.Code, error.GetProperty("code").GetString());
        Assert.Equal("Solicitud inválida", error.GetProperty("message").GetString());

        var details = response.GetProperty("details").EnumerateArray()
            .Select(element => new ErrorDetail(
                element.GetProperty("code").GetString(),
                element.GetProperty("message").GetString()))
            .ToArray();

        Assert.Single(details);
        Assert.Equal(DomainErrors.Product.IdRequired.Code, details[0].Code);
        Assert.Equal(DomainErrors.Product.IdRequired.Message, details[0].Message);
    }

    [Fact]
    public async Task Should_convert_not_found_exception_into_not_found_response()
    {
        static Task Next(HttpContext _) => throw new DomainNotFoundException(DomainErrors.Product.NotFound(Guid.Empty));

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        var error = response.GetProperty("error");
        Assert.Equal(DomainErrorCodes.NotFound, error.GetProperty("code").GetString());
        Assert.Equal("Recurso no encontrado", error.GetProperty("message").GetString());
        Assert.Equal(DomainErrors.Product.NotFound(Guid.Empty).Message, response.GetProperty("details").GetString());
    }

    [Fact]
    public async Task Should_convert_unhandled_exception_into_internal_server_error_response()
    {
        static Task Next(HttpContext _) => throw new InvalidOperationException("Something went terribly wrong.");

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        var error = response.GetProperty("error");
        Assert.Equal(ApiErrorCodes.Unexpected, error.GetProperty("code").GetString());
        Assert.Equal("Error interno del servidor", error.GetProperty("message").GetString());
        Assert.Equal("Ocurrió un error inesperado.", response.GetProperty("details").GetString());
    }

    [Fact]
    public async Task Should_include_trace_identifier_in_response()
    {
        const string ExpectedTraceIdentifier = "trace-id-123";

        Task Next(HttpContext _) => throw new DomainNotFoundException();

        var (context, response) = await InvokeMiddlewareAsync(Next, ExpectedTraceIdentifier).ConfigureAwait(false);

        Assert.Equal(ExpectedTraceIdentifier, response.GetProperty("traceId").GetString());
    }

    private static async Task<(DefaultHttpContext Context, JsonElement Response)> InvokeMiddlewareAsync(
        Func<HttpContext, Task> next,
        string? traceIdentifier = null)
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = traceIdentifier ?? Guid.NewGuid().ToString(),
        };
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context).ConfigureAwait(false);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body).ConfigureAwait(false);

        return (context, document.RootElement.Clone());
    }

    private sealed record ErrorDetail(string? Code, string? Message);
}
