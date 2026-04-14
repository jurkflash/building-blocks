namespace Pokok.BuildingBlocks.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a domain invariant or business rule is violated.
    /// Use this for domain-specific errors to distinguish them from infrastructure failures.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
