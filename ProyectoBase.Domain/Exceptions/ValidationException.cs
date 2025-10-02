namespace ProyectoBase.Domain.Exceptions
{
    /// <summary>
    /// Represents errors that occur when the domain detects invalid data.
    /// </summary>
    public class ValidationException : DomainException
    {
        private const string DefaultMessage = "The provided data is not valid.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
