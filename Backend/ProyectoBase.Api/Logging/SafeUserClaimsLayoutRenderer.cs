using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace ProyectoBase.Api.Logging;

/// <summary>
/// Layout renderer that exposes authenticated user claims without leaking personal information.
/// </summary>
[LayoutRenderer("safe-user-claims")]
public sealed class SafeUserClaimsLayoutRenderer : AspNetLayoutRendererBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Appends sanitized claim information to the provided string builder.
    /// </summary>
    /// <param name="builder">The builder receiving the rendered output.</param>
    /// <param name="logEvent">The log event being processed.</param>
    protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
    {
        var claimsByType = GetClaimTypeCounts(HttpContextAccessor?.HttpContext);
        if (claimsByType.Count == 0)
        {
            return;
        }

        builder.Append(JsonSerializer.Serialize(claimsByType, JsonOptions));
    }

    /// <summary>
    /// Aggregates authenticated user claims by type masking their actual values.
    /// </summary>
    /// <param name="httpContext">The HTTP context supplying the claims.</param>
    /// <returns>A dictionary containing claim type counts.</returns>
    internal static IReadOnlyDictionary<string, int> GetClaimTypeCounts(HttpContext? httpContext)
    {
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return EmptyClaims;
        }

        var claimsByType = httpContext.User.Claims
            .GroupBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        return claimsByType.Count == 0
            ? EmptyClaims
            : claimsByType;
    }

    /// <summary>
    /// Gets a reusable empty dictionary instance to avoid unnecessary allocations.
    /// </summary>
    private static IReadOnlyDictionary<string, int> EmptyClaims { get; } =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
}
