namespace ProyectoBase.Domain.Exceptions
{
    /// <summary>
    /// Represents errors that occur when a requested resource cannot be found in the domain.
    /// </summary>
    public class NotFoundException : DomainException
    {
        private const string DefaultMessage = "The requested resource was not found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        public NotFoundException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
