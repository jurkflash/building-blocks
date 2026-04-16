namespace Pokok.BuildingBlocks.Cqrs.Validation
{
    /// <summary>
    /// Exception thrown when one or more validation errors are detected during command or query validation.
    /// Contains an <see cref="Errors"/> collection with all accumulated validation messages.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Gets the collection of validation error messages.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with the specified errors.
        /// </summary>
        /// <param name="errors">The validation error messages.</param>
        public ValidationException(IEnumerable<string> errors): base("Validation failed.")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }
}
