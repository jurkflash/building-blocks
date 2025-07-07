using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class ValidatingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _inner;
        private readonly IEnumerable<IValidator<TQuery>> _validators;

        public ValidatingQueryHandler(
            IQueryHandler<TQuery, TResult> inner,
            IEnumerable<IValidator<TQuery>> validators)
        {
            _inner = inner;
            _validators = validators;
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            foreach (var validator in _validators)
            {
                try
                {
                    validator.Validate(query);
                }
                catch (ValidationException ex)
                {
                    errors.AddRange(ex.Errors);
                }
            }

            if (errors.Any())
                throw new ValidationException(errors);

            return await _inner.HandleAsync(query, cancellationToken);
        }
    }
}
