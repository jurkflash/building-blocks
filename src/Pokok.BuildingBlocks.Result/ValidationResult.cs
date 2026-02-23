namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Represents the outcome of a validation operation that may contain multiple errors.
    /// </summary>
    public sealed class ValidationResult : Result
    {
        private ValidationResult(Error[] errors)
            : base(false, errors.Length > 0 ? errors[0] : Error.None)
        {
            Errors = errors;
        }

        private ValidationResult()
            : base(true, Error.None)
        {
            Errors = Array.Empty<Error>();
        }

        /// <summary>
        /// Gets all validation errors.
        /// </summary>
        public IReadOnlyList<Error> Errors { get; }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        public static new ValidationResult Success() => new();

        /// <summary>
        /// Creates a failed validation result with the specified errors.
        /// </summary>
        public static ValidationResult WithErrors(params Error[] errors)
        {
            if (errors.Length == 0)
                throw new ArgumentException("At least one error is required.", nameof(errors));

            return new ValidationResult(errors);
        }
    }
}
