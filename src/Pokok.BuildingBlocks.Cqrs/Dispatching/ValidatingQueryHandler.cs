using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;
using System.Diagnostics;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Decorator that runs all registered <see cref="IValidator{T}"/> instances before delegating to the inner query handler.
    /// Accumulates all validation errors and throws <see cref="ValidationException"/> if any validation fails.
    /// </summary>
    /// <typeparam name="TQuery">The query type being validated and handled.</typeparam>
    /// <typeparam name="TResult">The return type of the query handler.</typeparam>
    public class ValidatingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _inner;
        private readonly IEnumerable<IValidator<TQuery>> _validators;
        private readonly ILogger<ValidatingQueryHandler<TQuery, TResult>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingQueryHandler{TQuery, TResult}"/> class.
        /// </summary>
        /// <param name="inner">The inner query handler to delegate to after validation.</param>
        /// <param name="validators">The validators to run before handling.</param>
        /// <param name="logger">The logger instance.</param>
        public ValidatingQueryHandler(
            IQueryHandler<TQuery, TResult> inner,
            IEnumerable<IValidator<TQuery>> validators,
            ILogger<ValidatingQueryHandler<TQuery, TResult>> logger)
        {
            _inner = inner;
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// Validates the query and delegates to the inner handler if validation passes.
        /// </summary>
        /// <param name="query">The query to validate and handle.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the query.</returns>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            List<string> errors = [];

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

            if (errors.Count > 0)
            {
                _logger.LogError("Query {QueryType} failed validation: {Errors}", typeof(TQuery).Name, string.Join("; ", errors));

                throw new ValidationException(errors);
            }

            _logger.LogDebug("Validation passed for query {QueryType}", typeof(TQuery).Name);
            return await _inner.HandleAsync(query, cancellationToken);
        }
    }
}
