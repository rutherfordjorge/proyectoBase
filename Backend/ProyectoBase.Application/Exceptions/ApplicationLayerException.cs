using System;

namespace ProyectoBase.Api.Application.Exceptions;

/// <summary>
/// Representa un error inesperado ocurrido dentro de la capa de aplicación.
/// </summary>
public class ApplicationLayerException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ApplicationLayerException"/> con un mensaje específico.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    public ApplicationLayerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ApplicationLayerException"/> con un mensaje y una excepción interna.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    /// <param name="innerException">La excepción que causó el error actual.</param>
    public ApplicationLayerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
