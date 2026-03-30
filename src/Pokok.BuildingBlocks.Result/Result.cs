namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Represents the outcome of an operation that does not return a value.
    /// Supports railway-oriented programming by chaining success/failure paths.
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("A successful result cannot have an error.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("A failed result must have an error.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result Success() => new(true, Error.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        public static Result Failure(Error error) => new(false, error);

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        public static Result<TValue> Success<TValue>(TValue value) =>
            new(value, true, Error.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        public static Result<TValue> Failure<TValue>(Error error) =>
            new(default, false, error);

        /// <summary>
        /// Executes the appropriate action based on the result state.
        /// </summary>
        public Result Match(Action onSuccess, Action<Error> onFailure)
        {
            if (IsSuccess)
                onSuccess();
            else
                onFailure(Error);

            return this;
        }

        /// <summary>
        /// Maps a successful result to a new result using the specified function.
        /// If the result is a failure, the error is propagated.
        /// </summary>
        public Result Bind(Func<Result> func) =>
            IsSuccess ? func() : this;

        /// <summary>
        /// Maps a successful result to a new typed result using the specified function.
        /// If the result is a failure, the error is propagated.
        /// </summary>
        public Result<TValue> Bind<TValue>(Func<Result<TValue>> func) =>
            IsSuccess ? func() : Result<TValue>.Failure(Error);

        /// <summary>
        /// Executes the action if the result is successful. Returns the original result.
        /// </summary>
        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
                action();

            return this;
        }

        /// <summary>
        /// Executes the action if the result is a failure. Returns the original result.
        /// </summary>
        public Result OnFailure(Action<Error> action)
        {
            if (IsFailure)
                action(Error);

            return this;
        }
    }
}
