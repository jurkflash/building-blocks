namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Represents the outcome of a validation operation that returns a value
    /// and may contain multiple errors.
    /// </summary>
    public sealed class ValidationResult<TValue> : Result<TValue>
    {
        private ValidationResult(TValue value)
            : base(value, true, Error.None)
        {
            Errors = Array.Empty<Error>();
        }

        private ValidationResult(Error[] errors)
            : base(default, false, errors.Length > 0 ? errors[0] : Error.None)
        {
            Errors = errors;
        }

        /// <summary>
        /// Gets all validation errors.
        /// </summary>
        public IReadOnlyList<Error> Errors { get; }

        /// <summary>
        /// Creates a successful validation result with the specified value.
        /// </summary>
        public static new ValidationResult<TValue> Success(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a failed validation result with the specified errors.
        /// </summary>
        public static ValidationResult<TValue> WithErrors(params Error[] errors)
        {
            if (errors.Length == 0)
                throw new ArgumentException("At least one error is required.", nameof(errors));

            return new ValidationResult<TValue>(errors);
        }
    }
}
