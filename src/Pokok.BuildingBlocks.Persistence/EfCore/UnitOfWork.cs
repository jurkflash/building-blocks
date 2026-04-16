using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence
{
    /// <summary>
    /// Default <see cref="IUnitOfWork"/> implementation. Saves all pending changes via EF Core,
    /// then extracts domain events from <see cref="IAggregateRoot"/> entities and dispatches them.
    /// Domain events are dispatched AFTER the database transaction commits.
    /// </summary>
    /// <typeparam name="TContext">The application's <see cref="DbContext"/> type.</typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IDomainEventDispatcher? _domainEventDispatcher;
        private readonly ILogger<UnitOfWork<TContext>> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="UnitOfWork{TContext}"/>.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        /// <param name="domainEventDispatcher">Optional dispatcher for domain events raised by aggregates.</param>
        /// <param name="logger">Optional logger for save and dispatch operations.</param>
        public UnitOfWork(TContext context, IDomainEventDispatcher? domainEventDispatcher = null, ILogger<UnitOfWork<TContext>>? logger = null)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
            _logger = logger ?? NullLogger<UnitOfWork<TContext>>.Instance;
        }

        /// <inheritdoc />
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes in {Context}", typeof(TContext).Name);

            var result = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Result} changes saved successfully in {Context}", result, typeof(TContext).Name);

            if (_domainEventDispatcher is null) 
                return result;

            var aggregates = _context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAggregateRoot)
                .Select(e => (IAggregateRoot)e.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();

            aggregates.ForEach(a => a.ClearDomainEvents());

            if (domainEvents.Count > 0)
            {
                _logger.LogInformation("Dispatching {Count} domain events from {Context}", domainEvents.Count, typeof(TContext).Name);
                await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }

            return result;
        }
    }
}
