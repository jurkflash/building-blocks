namespace Pokok.BuildingBlocks.Cqrs.Validation
{
    /// <summary>
    /// Exception thrown when one or more validation errors are detected during command or query validation.
    /// Contains an <see cref="Errors"/> collection with all accumulated validation messages.
    /// </summary>
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors): base("Validation failed.")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
