using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace ProyectoBase.Api.Api.Logging;

/// <summary>
/// Representa un layout renderer que emite los encabezados de la solicitud HTTP ocultando los valores sensibles.
/// </summary>
[LayoutRenderer("safe-request-headers")]
public sealed class SafeRequestHeadersLayoutRenderer : AspNetLayoutRendererBase
{
    private const string Mask = "***";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly ISet<string> SensitiveHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Proxy-Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key",
        "X-Amzn-Trace-Id",
    };

    private static readonly Regex[] SensitiveValuePatterns =
    {
        new("(?i)(token|secret|password|apikey|sessionid)[^a-z0-9]*[a-z0-9]+", RegexOptions.Compiled),
        new("(?i)bearer\\s+[a-z0-9\-_.]+", RegexOptions.Compiled),
        new("(?i)basic\\s+[a-z0-9+/=]+", RegexOptions.Compiled),
    };

    /// <summary>
    /// Agrega los encabezados saneados al generador del registro.
    /// </summary>
    /// <param name="builder">El generador que recibe la salida renderizada.</param>
    /// <param name="logEvent">El evento de registro que se está procesando.</param>
    protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
    {
        var sanitized = SanitizeHeaders(HttpContextAccessor?.HttpContext);
        if (sanitized.Count == 0)
        {
            return;
        }

        builder.Append(JsonSerializer.Serialize(sanitized, JsonOptions));
    }

    /// <summary>
    /// Produce una instantánea saneada de los encabezados presentes en el contexto HTTP suministrado.
    /// </summary>
    /// <param name="httpContext">El contexto HTTP actual.</param>
    /// <returns>Un diccionario con la información sensible enmascarada.</returns>
    internal static IReadOnlyDictionary<string, string> SanitizeHeaders(HttpContext? httpContext)
    {
        if (httpContext?.Request?.Headers is null || httpContext.Request.Headers.Count == 0)
        {
            return EmptyHeaders;
        }

        var sanitized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in httpContext.Request.Headers)
        {
            sanitized[header.Key] = SanitizeHeader(header.Key, header.Value);
        }

        return sanitized.Count == 0
            ? EmptyHeaders
            : sanitized;
    }

    /// <summary>
    /// Obtiene una instancia reutilizable de diccionario vacío para evitar asignaciones innecesarias.
    /// </summary>
    private static IReadOnlyDictionary<string, string> EmptyHeaders { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private static string SanitizeHeader(string name, StringValues value)
    {
        if (SensitiveHeaders.Contains(name))
        {
            return Mask;
        }

        var concatenated = string.Join(", ", value.ToArray());
        if (string.IsNullOrWhiteSpace(concatenated))
        {
            return concatenated;
        }

        if (SensitiveValuePatterns.Any(pattern => pattern.IsMatch(concatenated)))
        {
            return Mask;
        }

        return concatenated;
    }
}
