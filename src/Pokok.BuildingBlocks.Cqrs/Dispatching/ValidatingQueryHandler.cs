using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;
using System.Diagnostics;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class ValidatingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _inner;
        private readonly IEnumerable<IValidator<TQuery>> _validators;
        private readonly ILogger<ValidatingQueryHandler<TQuery, TResult>> _logger;

        public ValidatingQueryHandler(
            IQueryHandler<TQuery, TResult> inner,
            IEnumerable<IValidator<TQuery>> validators,
            ILogger<ValidatingQueryHandler<TQuery, TResult>> logger)
        {
            _inner = inner;
            _validators = validators;
            _logger = logger;
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
                    _logger.LogWarning("Validation failed for query {QueryType}: {Errors}", typeof(TQuery).Name, string.Join("; ", ex.Errors));

                    errors.AddRange(ex.Errors);
                }
            }

            if (errors.Any())
            {
                _logger.LogError("Query {QueryType} failed validation: {Errors}", typeof(TQuery).Name, string.Join("; ", errors));

                throw new ValidationException(errors);
            }

            _logger.LogDebug("Validation passed for query {QueryType}", typeof(TQuery).Name);
            return await _inner.HandleAsync(query, cancellationToken);
        }
    }
}
