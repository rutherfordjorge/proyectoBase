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
/// Representa un layout renderer que expone los claims del usuario autenticado sin revelar información personal.
/// </summary>
[LayoutRenderer("safe-user-claims")]
public sealed class SafeUserClaimsLayoutRenderer : AspNetLayoutRendererBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Agrega la información saneada de los claims al generador de cadenas proporcionado.
    /// </summary>
    /// <param name="builder">El generador que recibe la salida renderizada.</param>
    /// <param name="logEvent">El evento de registro que se está procesando.</param>
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
    /// Agrupa los claims del usuario autenticado por tipo enmascarando sus valores reales.
    /// </summary>
    /// <param name="httpContext">El contexto HTTP que proporciona los claims.</param>
    /// <returns>Un diccionario que contiene la cantidad de claims por tipo.</returns>
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
    /// Obtiene una instancia reutilizable de diccionario vacío para evitar asignaciones innecesarias.
    /// </summary>
    private static IReadOnlyDictionary<string, int> EmptyClaims { get; } =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
}
