namespace ProyectoBase.Api.Swagger;

/// <summary>
/// Representa el contrato de error estandarizado que devuelve la API.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Obtiene el identificador único asociado a la solicitud.
    /// </summary>
    public required string TraceId { get; init; }

    /// <summary>
    /// Obtiene el código de estado HTTP devuelto por la API.
    /// </summary>
    public required int Status { get; init; }

    /// <summary>
    /// Obtiene una descripción breve del error.
    /// </summary>
    public required string Error { get; init; }

    /// <summary>
    /// Obtiene detalles adicionales del error cuando están disponibles.
    /// </summary>
    public object? Details { get; init; }
}
