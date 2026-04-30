using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Decorator that runs all registered <see cref="IValidator{T}"/> instances before delegating to the inner command handler.
    /// Accumulates all validation errors and throws <see cref="ValidationException"/> if any validation fails.
    /// </summary>
    /// <typeparam name="TCommand">The command type being validated and handled.</typeparam>
    /// <typeparam name="TResult">The return type of the command handler.</typeparam>
    public class ValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _inner;
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        private readonly ILogger<ValidatingCommandHandler<TCommand, TResult>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingCommandHandler{TCommand, TResult}"/> class.
        /// </summary>
        /// <param name="inner">The inner command handler to delegate to after validation.</param>
        /// <param name="validators">The validators to run before handling.</param>
        /// <param name="logger">The logger instance.</param>
        public ValidatingCommandHandler(
            ICommandHandler<TCommand, TResult> inner,
            IEnumerable<IValidator<TCommand>> validators,
            ILogger<ValidatingCommandHandler<TCommand, TResult>> logger)
        {
            _inner = inner;
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// Validates the command and delegates to the inner handler if validation passes.
        /// </summary>
        /// <param name="command">The command to validate and handle.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the command.</returns>
        public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            List<string> errors = [];

            foreach (var validator in _validators)
            {
                try
                {
                    validator.Validate(command);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning("Validation failed for {CommandType} with {ErrorCount} error(s): {Errors}", typeof(TCommand).Name, ex.Errors.Count(), string.Join("; ", ex.Errors));

                    errors.AddRange(ex.Errors);
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogError("Command {CommandType} failed validation: {Errors}", typeof(TCommand).Name, string.Join("; ", errors));
                throw new ValidationException(errors);
            }

            _logger.LogDebug("Validation passed for {CommandType}", typeof(TCommand).Name);
            return await _inner.HandleAsync(command, cancellationToken);
        }
    }
}
