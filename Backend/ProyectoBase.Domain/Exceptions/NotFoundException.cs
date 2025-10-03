using ProyectoBase.Api.Domain;

namespace ProyectoBase.Api.Domain.Exceptions
{
    /// <summary>
    /// Representa errores que ocurren cuando no se encuentra un recurso solicitado en el dominio.
    /// </summary>
    public class NotFoundException : DomainException
    {
        private static readonly DomainError DefaultError = DomainErrors.General.NotFound;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotFoundException"/>.
        /// </summary>
        public NotFoundException()
            : base(DefaultError)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotFoundException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public NotFoundException(string message)
            : base(DomainErrorCodes.NotFound, message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotFoundException"/> con un descriptor de error específico.
        /// </summary>
        /// <param name="error">El descriptor que describe el error.</param>
        public NotFoundException(DomainError error)
            : base(error)
        {
        }
    }
}
