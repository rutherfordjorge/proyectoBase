using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ProyectoBase.Api.Middleware;
using Xunit;

namespace ProyectoBase.Application.Tests.Middleware;

/// <summary>
/// Contains unit tests for <see cref="ValidationExceptionHandlingMiddleware"/>.
/// </summary>
public class ValidationExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Should_convert_fluent_validation_exception_into_bad_request_response()
    {
        static Task Next(HttpContext _) => throw new ValidationException(new[]
        {
            new ValidationFailure("Name", "The name field is required."),
            new ValidationFailure("Name", "The name field is required."),
            new ValidationFailure("Price", "The price must be greater than zero."),
        });

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ValidationExceptionHandlingMiddleware(Next, NullLogger<ValidationExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);

        var errors = response.GetProperty("errors").EnumerateArray().Select(element => element.GetString()).ToArray();

        Assert.Equal(2, errors.Length);
        Assert.Contains("The name field is required.", errors);
        Assert.Contains("The price must be greater than zero.", errors);
    }

    [Fact]
    public async Task Should_convert_domain_validation_exception_into_bad_request_response()
    {
        static Task Next(HttpContext _) => throw new ProyectoBase.Domain.Exceptions.ValidationException("Domain error.");

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ValidationExceptionHandlingMiddleware(Next, NullLogger<ValidationExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);

        var errors = response.GetProperty("errors").EnumerateArray().Select(element => element.GetString()).ToArray();

        Assert.Single(errors);
        Assert.Contains("Domain error.", errors);
    }
}
