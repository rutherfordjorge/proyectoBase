using ProyectoBase.Api.Domain;

namespace ProyectoBase.Api.Domain.Exceptions
{
    /// <summary>
    /// Representa errores que ocurren cuando el dominio detecta datos no válidos.
    /// </summary>
    public class ValidationException : DomainException
    {
        private static readonly DomainError DefaultError = DomainErrors.General.Validation;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationException"/>.
        /// </summary>
        public ValidationException()
            : base(DefaultError)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public ValidationException(string message)
            : base(DomainErrorCodes.Validation, message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationException"/> con un descriptor de error específico.
        /// </summary>
        /// <param name="error">El descriptor que describe el error.</param>
        public ValidationException(DomainError error)
            : base(error)
        {
        }
    }
}
