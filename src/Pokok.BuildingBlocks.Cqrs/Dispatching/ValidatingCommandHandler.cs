using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class ValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _inner;
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        private readonly ILogger<ValidatingCommandHandler<TCommand, TResult>> _logger;

        public ValidatingCommandHandler(
            ICommandHandler<TCommand, TResult> inner,
            IEnumerable<IValidator<TCommand>> validators,
            ILogger<ValidatingCommandHandler<TCommand, TResult>> logger)
        {
            _inner = inner;
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

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

            if (errors.Any())
            {
                _logger.LogError("Command {CommandType} failed validation: {Errors}", typeof(TCommand).Name, string.Join("; ", errors));
                throw new ValidationException(errors);
            }

            _logger.LogDebug("Validation passed for {CommandType}", typeof(TCommand).Name);
            return await _inner.HandleAsync(command, cancellationToken);
        }
    }
}
