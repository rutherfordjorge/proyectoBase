using System;
using ProyectoBase.Api.Domain;

namespace ProyectoBase.Api.Domain.Exceptions
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
            : this(DomainErrorCodes.General, message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un mensaje de error específico y una referencia a la excepción interna que causó esta excepción.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que provocó la excepción actual.</param>
        public DomainException(string message, Exception innerException)
            : this(DomainErrorCodes.General, message, innerException)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un código y mensaje específicos.
        /// </summary>
        /// <param name="code">El código de error asociado a la excepción.</param>
        /// <param name="message">El mensaje que describe el error.</param>
        public DomainException(string code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> con un código, un mensaje y una excepción interna.
        /// </summary>
        /// <param name="code">El código de error asociado a la excepción.</param>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que provocó la excepción actual.</param>
        public DomainException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> a partir de un descriptor de error.
        /// </summary>
        /// <param name="error">El descriptor que contiene el código y el mensaje localizados.</param>
        public DomainException(DomainError error)
            : this(error.Code, error.Message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/> a partir de un descriptor de error y una excepción interna.
        /// </summary>
        /// <param name="error">El descriptor que contiene el código y el mensaje localizados.</param>
        /// <param name="innerException">La excepción que provocó la excepción actual.</param>
        public DomainException(DomainError error, Exception innerException)
            : this(error.Code, error.Message, innerException)
        {
        }

        /// <summary>
        /// Obtiene el código de error asociado a la excepción.
        /// </summary>
        public string Code { get; }
    }
}
