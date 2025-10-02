namespace ProyectoBase.Domain.Exceptions
{
    /// <summary>
    /// Representa errores que ocurren cuando el dominio detecta datos no válidos.
    /// </summary>
    public class ValidationException : DomainException
    {
        private const string DefaultMessage = "Los datos proporcionados no son válidos.";

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationException"/>.
        /// </summary>
        public ValidationException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
