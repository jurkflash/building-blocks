namespace Pokok.BuildingBlocks.Result
{
    /// <summary>
    /// Extension methods for working with async Result operations.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Awaits the task and maps the value of a successful result.
        /// </summary>
        public static async Task<Result<TResult>> Map<TValue, TResult>(
            this Task<Result<TValue>> resultTask,
            Func<TValue, TResult> map)
        {
            var result = await resultTask;
            return result.Map(map);
        }

        /// <summary>
        /// Awaits the task and binds the result to a new result.
        /// </summary>
        public static async Task<Result<TResult>> Bind<TValue, TResult>(
            this Task<Result<TValue>> resultTask,
            Func<TValue, Result<TResult>> func)
        {
            var result = await resultTask;
            return result.Bind(func);
        }

        /// <summary>
        /// Awaits the task and matches the result to one of two functions.
        /// </summary>
        public static async Task<TResult> Match<TValue, TResult>(
            this Task<Result<TValue>> resultTask,
            Func<TValue, TResult> onSuccess,
            Func<Error, TResult> onFailure)
        {
            var result = await resultTask;
            return result.Match(onSuccess, onFailure);
        }

        /// <summary>
        /// Awaits the task and matches the result to one of two actions.
        /// </summary>
        public static async Task<Result> Match(
            this Task<Result> resultTask,
            Action onSuccess,
            Action<Error> onFailure)
        {
            var result = await resultTask;
            return result.Match(onSuccess, onFailure);
        }

        /// <summary>
        /// Combines multiple results into a single result. Succeeds only if all results succeed.
        /// </summary>
        public static Result Combine(params Result[] results)
        {
            var firstFailure = results.FirstOrDefault(r => r.IsFailure);
            return firstFailure ?? Result.Success();
        }

        /// <summary>
        /// Combines multiple results, collecting all errors into a <see cref="ValidationResult"/>.
        /// </summary>
        public static ValidationResult CombineAsValidation(params Result[] results)
        {
            var errors = results
                .Where(r => r.IsFailure)
                .Select(r => r.Error)
                .ToArray();

            return errors.Length > 0
                ? ValidationResult.WithErrors(errors)
                : ValidationResult.Success();
        }

        /// <summary>
        /// Converts a nullable value to a Result, using the specified error if the value is null.
        /// </summary>
        public static Result<TValue> ToResult<TValue>(this TValue? value, Error error)
            where TValue : class =>
            value is not null ? Result<TValue>.Success(value) : Result<TValue>.Failure(error);

        /// <summary>
        /// Converts a nullable value type to a Result, using the specified error if the value is null.
        /// </summary>
        public static Result<TValue> ToResult<TValue>(this TValue? value, Error error)
            where TValue : struct =>
            value.HasValue ? Result<TValue>.Success(value.Value) : Result<TValue>.Failure(error);
    }
}
