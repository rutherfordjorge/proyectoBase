using System.Diagnostics;
using System.Text.Json;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProyectoBase.Domain.Exceptions;

namespace ProyectoBase.Api.Middlewares;

/// <summary>
/// Middleware responsible for translating unhandled exceptions into standardized HTTP responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger used to emit diagnostic information.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the current <see cref="HttpContext"/> instance.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogError(exception, "The response has already started, the exception middleware cannot handle the error.");
            throw;
        }

        var mapping = MapException(exception);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = mapping.StatusCode;
        context.Response.ContentType = "application/json";

        _logger.Log(mapping.LogLevel, exception, mapping.LogMessage, context.Request.Path);

        var payload = new
        {
            traceId,
            status = mapping.StatusCode,
            error = mapping.Error,
            details = mapping.Details,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions)).ConfigureAwait(false);
    }

    private static ExceptionMapping MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException notFound => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status404NotFound,
                Error = ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound)!,
                Details = string.IsNullOrWhiteSpace(notFound.Message)
                    ? "The requested resource was not found."
                    : notFound.Message,
                LogLevel = LogLevel.Warning,
                LogMessage = "A not found error occurred while processing {Path}.",
            },
            Domain.Exceptions.ValidationException domainValidation => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Error = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)!,
                Details = GetDomainValidationDetails(domainValidation),
                LogLevel = LogLevel.Warning,
                LogMessage = "A domain validation error occurred while processing {Path}.",
            },
            ValidationException fluentValidation => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Error = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)!,
                Details = GetFluentValidationDetails(fluentValidation),
                LogLevel = LogLevel.Warning,
                LogMessage = "A validation error occurred while processing {Path}.",
            },
            _ => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Error = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)!,
                Details = "An unexpected error occurred.",
                LogLevel = LogLevel.Error,
                LogMessage = "An unexpected error occurred while processing {Path}.",
            },
        };
    }

    private static IReadOnlyCollection<string> GetDomainValidationDetails(Domain.Exceptions.ValidationException exception)
    {
        var message = string.IsNullOrWhiteSpace(exception.Message)
            ? "The provided data is not valid."
            : exception.Message;

        return new[] { message };
    }

    private static IReadOnlyCollection<string> GetFluentValidationDetails(ValidationException exception)
    {
        return exception.Errors
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private sealed record ExceptionMapping
    {
        public required int StatusCode { get; init; }

        public required string Error { get; init; }

        public required object Details { get; init; }

        public required LogLevel LogLevel { get; init; }

        public required string LogMessage { get; init; }
    }
}
