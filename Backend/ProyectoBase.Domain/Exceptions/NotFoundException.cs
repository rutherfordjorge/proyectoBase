namespace ProyectoBase.Api.Domain.Exceptions
{
    /// <summary>
    /// Representa errores que ocurren cuando no se encuentra un recurso solicitado en el dominio.
    /// </summary>
    public class NotFoundException : DomainException
    {
        private const string DefaultMessage = "El recurso solicitado no fue encontrado.";

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotFoundException"/>.
        /// </summary>
        public NotFoundException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotFoundException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
