namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Represents a structured error with a code and human-readable description.
    /// </summary>
    public sealed record Error(string Code, string Description)
    {
        /// <summary>
        /// Represents no error. Used as the default for successful results.
        /// </summary>
        public static readonly Error None = new(string.Empty, string.Empty);

        /// <summary>
        /// Creates a not found error.
        /// </summary>
        public static Error NotFound(string code, string description) =>
            new(code, description);

        /// <summary>
        /// Creates a validation error.
        /// </summary>
        public static Error Validation(string code, string description) =>
            new(code, description);

        /// <summary>
        /// Creates a conflict error.
        /// </summary>
        public static Error Conflict(string code, string description) =>
            new(code, description);

        /// <summary>
        /// Creates a failure error.
        /// </summary>
        public static Error Failure(string code, string description) =>
            new(code, description);

        public override string ToString() => $"{Code}: {Description}";
    }
}
