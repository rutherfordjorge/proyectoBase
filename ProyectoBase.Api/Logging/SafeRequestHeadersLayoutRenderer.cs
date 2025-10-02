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

namespace ProyectoBase.Api.Logging;

/// <summary>
/// Layout renderer that outputs HTTP request headers while masking sensitive values.
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
    /// Appends the sanitized headers to the log builder.
    /// </summary>
    /// <param name="builder">The builder receiving the rendered output.</param>
    /// <param name="logEvent">The log event being processed.</param>
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
    /// Produces a sanitized snapshot of the headers contained in the provided HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>A dictionary with sensitive information masked.</returns>
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
    /// Gets a reusable empty dictionary instance to avoid unnecessary allocations.
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
