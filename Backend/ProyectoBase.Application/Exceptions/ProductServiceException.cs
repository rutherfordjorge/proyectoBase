using System;

namespace ProyectoBase.Application.Exceptions;

/// <summary>
/// Representa errores inesperados que se producen al operar con productos en la capa de aplicación.
/// </summary>
public sealed class ProductServiceException : ApplicationLayerException
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ProductServiceException"/>.
    /// </summary>
    /// <param name="message">El mensaje descriptivo del error.</param>
    /// <param name="innerException">La excepción original que produjo el error.</param>
    public ProductServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
