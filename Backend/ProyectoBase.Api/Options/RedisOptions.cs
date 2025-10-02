namespace ProyectoBase.Api.Api.Options;

/// <summary>
/// Representa la configuración necesaria para conectar la aplicación a un servidor de Redis.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Nombre de la sección de configuración que contiene las opciones de Redis.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// Cadena de conexión utilizada para establecer la comunicación con la instancia de Redis.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Identificador opcional que se antepone a las claves almacenadas en Redis.
    /// </summary>
    public string InstanceName { get; set; } = string.Empty;
}
