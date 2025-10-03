namespace ProyectoBase.Api.Domain;

/// <summary>
/// Representa un descriptor de error reutilizable compuesto por un código y un mensaje localizado.
/// </summary>
/// <param name="Code">El identificador único y estable del error.</param>
/// <param name="Message">El mensaje localizado asociado al error.</param>
public readonly record struct DomainError(string Code, string Message);
