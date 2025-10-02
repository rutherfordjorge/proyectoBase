namespace ProyectoBase.Api.Swagger;

/// <summary>
/// Represents the standardized error contract returned by the API.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Gets the unique identifier associated with the request.
    /// </summary>
    public required string TraceId { get; init; }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public required int Status { get; init; }

    /// <summary>
    /// Gets a short description of the error.
    /// </summary>
    public required string Error { get; init; }

    /// <summary>
    /// Gets additional error details when available.
    /// </summary>
    public object? Details { get; init; }
}
