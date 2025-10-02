using System.Diagnostics;
using System.Text.Json;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProyectoBase.Domain.Exceptions;

namespace ProyectoBase.Api.Middlewares;

/// <summary>
/// Middleware responsable de traducir las excepciones no controladas en respuestas HTTP estandarizadas.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">El siguiente middleware en la canalización.</param>
    /// <param name="logger">El registrador utilizado para emitir información de diagnóstico.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Procesa la instancia actual de <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">El contexto HTTP.</param>
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
            _logger.LogError(exception, "La respuesta ya comenzó; el middleware de excepciones no puede gestionar el error.");
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
                Error = "Recurso no encontrado",
                Details = string.IsNullOrWhiteSpace(notFound.Message)
                    ? "El recurso solicitado no fue encontrado."
                    : notFound.Message,
                LogLevel = LogLevel.Warning,
                LogMessage = "Se produjo un error de recurso no encontrado al procesar {Path}.",
            },
            Domain.Exceptions.ValidationException domainValidation => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Error = "Solicitud inválida",
                Details = GetDomainValidationDetails(domainValidation),
                LogLevel = LogLevel.Warning,
                LogMessage = "Se produjo un error de validación de dominio al procesar {Path}.",
            },
            ValidationException fluentValidation => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Error = "Solicitud inválida",
                Details = GetFluentValidationDetails(fluentValidation),
                LogLevel = LogLevel.Warning,
                LogMessage = "Se produjo un error de validación al procesar {Path}.",
            },
            _ => new ExceptionMapping
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Error = "Error interno del servidor",
                Details = "Ocurrió un error inesperado.",
                LogLevel = LogLevel.Error,
                LogMessage = "Ocurrió un error inesperado al procesar {Path}.",
            },
        };
    }

    private static IReadOnlyCollection<string> GetDomainValidationDetails(Domain.Exceptions.ValidationException exception)
    {
        var message = string.IsNullOrWhiteSpace(exception.Message)
            ? "Los datos proporcionados no son válidos."
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
