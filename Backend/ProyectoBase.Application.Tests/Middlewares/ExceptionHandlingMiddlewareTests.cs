using System.Linq;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ProyectoBase.Api.Middlewares;
using DomainNotFoundException = ProyectoBase.Domain.Exceptions.NotFoundException;
using DomainValidationException = ProyectoBase.Domain.Exceptions.ValidationException;
using Xunit;

namespace ProyectoBase.Application.Tests.Middlewares;

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
        Assert.Equal("Bad Request", response.GetProperty("error").GetString());

        var details = response.GetProperty("details").EnumerateArray().Select(element => element!.GetString()).ToArray();

        Assert.Equal(2, details.Length);
        Assert.Contains("The name field is required.", details);
        Assert.Contains("The price must be greater than zero.", details);
    }

    [Fact]
    public async Task Should_convert_domain_validation_exception_into_bad_request_response()
    {
        static Task Next(HttpContext _) => throw new DomainValidationException("Domain validation failure.");

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("Bad Request", response.GetProperty("error").GetString());

        var details = response.GetProperty("details").EnumerateArray().Select(element => element!.GetString()).ToArray();

        Assert.Single(details);
        Assert.Equal("Domain validation failure.", details[0]);
    }

    [Fact]
    public async Task Should_convert_not_found_exception_into_not_found_response()
    {
        static Task Next(HttpContext _) => throw new DomainNotFoundException("Entity not found.");

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("Not Found", response.GetProperty("error").GetString());
        Assert.Equal("Entity not found.", response.GetProperty("details").GetString());
    }

    [Fact]
    public async Task Should_convert_unhandled_exception_into_internal_server_error_response()
    {
        static Task Next(HttpContext _) => throw new InvalidOperationException("Something went terribly wrong.");

        var (context, response) = await InvokeMiddlewareAsync(Next).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("Internal Server Error", response.GetProperty("error").GetString());
        Assert.Equal("An unexpected error occurred.", response.GetProperty("details").GetString());
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
}
