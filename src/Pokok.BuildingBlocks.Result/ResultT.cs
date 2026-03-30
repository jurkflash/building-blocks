namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Represents the outcome of an operation that returns a value of type <typeparamref name="TValue"/>.
    /// </summary>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of a successful result.
        /// Throws <see cref="InvalidOperationException"/> if the result is a failure.
        /// </summary>
        public TValue Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("Cannot access the value of a failed result.");

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        public static Result<TValue> Success(TValue value) =>
            new(value, true, Error.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        public new static Result<TValue> Failure(Error error) =>
            new(default, false, error);

        /// <summary>
        /// Implicitly converts a value to a successful result.
        /// </summary>
        public static implicit operator Result<TValue>(TValue value) =>
            Success(value);

        /// <summary>
        /// Implicitly converts an error to a failed result.
        /// </summary>
        public static implicit operator Result<TValue>(Error error) =>
            Failure(error);

        /// <summary>
        /// Executes the appropriate function based on the result state and returns the mapped value.
        /// </summary>
        public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
            IsSuccess ? onSuccess(Value) : onFailure(Error);

        /// <summary>
        /// Maps the value of a successful result to a new value.
        /// If the result is a failure, the error is propagated.
        /// </summary>
        public Result<TResult> Map<TResult>(Func<TValue, TResult> map) =>
            IsSuccess
                ? Result<TResult>.Success(map(Value))
                : Result<TResult>.Failure(Error);

        /// <summary>
        /// Maps a successful result to a new result using the specified function.
        /// If the result is a failure, the error is propagated.
        /// </summary>
        public Result<TResult> Bind<TResult>(Func<TValue, Result<TResult>> func) =>
            IsSuccess ? func(Value) : Result<TResult>.Failure(Error);

        /// <summary>
        /// Executes the action if the result is successful. Returns the original result.
        /// </summary>
        public Result<TValue> OnSuccess(Action<TValue> action)
        {
            if (IsSuccess)
                action(Value);

            return this;
        }

        /// <summary>
        /// Executes the action if the result is a failure. Returns the original result.
        /// </summary>
        public new Result<TValue> OnFailure(Action<Error> action)
        {
            if (IsFailure)
                action(Error);

            return this;
        }
    }
}
