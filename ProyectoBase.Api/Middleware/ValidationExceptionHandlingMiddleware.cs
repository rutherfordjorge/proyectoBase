using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DomainValidationException = ProyectoBase.Domain.Exceptions.ValidationException;

namespace ProyectoBase.Api.Middleware;

/// <summary>
/// Middleware that converts validation exceptions into standardized HTTP responses.
/// </summary>
public class ValidationExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger used to emit diagnostics information.</param>
    public ValidationExceptionHandlingMiddleware(RequestDelegate next, ILogger<ValidationExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the HTTP context and handles potential validation exceptions.
    /// </summary>
    /// <param name="context">The HTTP context to process.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            await HandleFluentValidationExceptionAsync(context, exception).ConfigureAwait(false);
        }
        catch (DomainValidationException exception)
        {
            await HandleDomainValidationExceptionAsync(context, exception).ConfigureAwait(false);
        }
    }

    private Task HandleFluentValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        _logger.LogWarning(exception, "A validation error occurred while processing {Path}.", context.Request.Path);

        return WriteResponseAsync(context, errors);
    }

    private Task HandleDomainValidationExceptionAsync(HttpContext context, DomainValidationException exception)
    {
        var message = string.IsNullOrWhiteSpace(exception.Message)
            ? "The provided data is not valid."
            : exception.Message;

        _logger.LogWarning(exception, "A domain validation error occurred while processing {Path}.", context.Request.Path);

        return WriteResponseAsync(context, new[] { message });
    }

    private static Task WriteResponseAsync(HttpContext context, IReadOnlyCollection<string> errors)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var payload = new { errors };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }
}
