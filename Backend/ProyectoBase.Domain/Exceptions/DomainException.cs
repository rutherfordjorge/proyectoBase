using System;

namespace ProyectoBase.Domain.Exceptions
{
    /// <summary>
    /// Representa errores que ocurren dentro de la capa de dominio.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public DomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un mensaje de error específico y una referencia a la excepción interna que causó esta excepción.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que provocó la excepción actual.</param>
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
