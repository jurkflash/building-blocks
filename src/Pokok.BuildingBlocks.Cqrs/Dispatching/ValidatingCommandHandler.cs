using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class ValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _inner;
        private readonly IEnumerable<IValidator<TCommand>> _validators;

        public ValidatingCommandHandler(
            ICommandHandler<TCommand, TResult> inner,
            IEnumerable<IValidator<TCommand>> validators)
        {
            _inner = inner;
            _validators = validators;
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
                    errors.AddRange(ex.Errors);
                }
            }

            if (errors.Any())
                throw new ValidationException(errors);

            return await _inner.HandleAsync(command, cancellationToken);
        }
    }
}
