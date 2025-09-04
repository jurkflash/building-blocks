using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IDomainEventDispatcher? _domainEventDispatcher;
        private readonly ILogger<UnitOfWork<TContext>> _logger;

        public UnitOfWork(TContext context, IDomainEventDispatcher? domainEventDispatcher = null, ILogger<UnitOfWork<TContext>>? logger = null)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
            _logger = logger ?? NullLogger<UnitOfWork<TContext>>.Instance;
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes in {Context}", typeof(TContext).Name);

            var result = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Result} changes saved successfully in {Context}", result, typeof(TContext).Name);

            if (_domainEventDispatcher is null) 
                return result;

            // Collect domain events from aggregates
            var aggregates = _context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is AggregateRoot<object>) // detect aggregates
                .Select(e => e.Entity as AggregateRoot<object>)
                .Where(e => e is not null)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a!.DomainEvents)
                .ToList();

            // Clear them from the aggregates
            aggregates.ForEach(a => a!.ClearDomainEvents());

            if (domainEvents.Count > 0)
                _logger.LogInformation("Dispatching {Count} domain events from {Context}", domainEvents.Count, typeof(TContext).Name);

            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

            return result;
        }
    }
}
