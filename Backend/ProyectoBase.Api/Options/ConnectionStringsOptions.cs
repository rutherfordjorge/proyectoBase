namespace ProyectoBase.Api.Api.Options;

/// <summary>
/// Representa la configuración de cadenas de conexión utilizadas por la aplicación.
/// </summary>
public class ConnectionStringsOptions
{
    /// <summary>
    /// Nombre de la sección de configuración que contiene las cadenas de conexión.
    /// </summary>
    public const string SectionName = "ConnectionStrings";

    /// <summary>
    /// Nombre de la cadena de conexión predeterminada que debe utilizar la aplicación.
    /// </summary>
    public const string DefaultConnectionName = "DefaultConnection";

    /// <summary>
    /// Cadena de conexión utilizada como valor predeterminado para acceder a la base de datos.
    /// </summary>
    public string DefaultConnection { get; set; } = string.Empty;
}
