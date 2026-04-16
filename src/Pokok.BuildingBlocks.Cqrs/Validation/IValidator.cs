namespace Pokok.BuildingBlocks.Cqrs.Validation
{
    /// <summary>
    /// Validates a request of type <typeparamref name="T"/> before it is handled.
    /// Implementations should throw <see cref="ValidationException"/> with descriptive error messages on failure.
    /// </summary>
    /// <typeparam name="T">The type of the request (command or query) to validate.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates the specified request, throwing <see cref="ValidationException"/> on failure.
        /// </summary>
        /// <param name="request">The request to validate.</param>
        void Validate(T request);
    }
}
