using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence.Base
{
    public class UnitOfWorkBase : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWorkBase(DbContext context, IDomainEventDispatcher domainEventDispatcher)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            // Save DB changes first
            var result = await _context.SaveChangesAsync(cancellationToken);

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

            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

            return result;
        }
    }
}
