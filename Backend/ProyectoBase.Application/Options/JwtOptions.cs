namespace ProyectoBase.Api.Application.Options;

/// <summary>
/// Representa la configuración necesaria para generar y validar tokens JWT.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Nombre de la sección de configuración que contiene los valores de JWT.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Obtiene o establece el emisor del token.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la audiencia esperada del token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la clave simétrica utilizada para firmar los tokens.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el tiempo de expiración del token de acceso en minutos.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Obtiene o establece el tiempo de expiración del token de actualización en días.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
