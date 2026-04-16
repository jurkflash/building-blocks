namespace Pokok.BuildingBlocks.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a domain invariant or business rule is violated.
    /// Use this for domain-specific errors to distinguish them from infrastructure failures.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DomainException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message describing the domain rule violation.</param>
        public DomainException(string message) : base(message) { }
    }
}
