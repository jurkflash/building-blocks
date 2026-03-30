using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Common;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using Pokok.BuildingBlocks.Persistence.EfCore;
using Pokok.BuildingBlocks.Persistence.Entities;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.EfCore;

internal class TestEntity : EntityBase { }

internal class TestDbContext : DbContextBase
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();

    public TestDbContext(DbContextOptions options, ICurrentUserService? currentUser = null)
        : base(options, currentUser)
    {
    }
}

internal class TestAggregateRoot : AggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public TestAggregateRoot(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public void RaiseTestEvent() => AddDomainEvent(new TestDomainEvent());
}

internal class TestDomainEvent : DomainEventBase { }

internal class TestAggregateContext : DbContext
{
    public DbSet<TestAggregateRoot> Aggregates => Set<TestAggregateRoot>();

    public TestAggregateContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestAggregateRoot>().Ignore(a => a.DomainEvents);
    }
}

public class DbContextBaseTests
{
    private static TestDbContext CreateContext(ICurrentUserService? currentUser = null)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options, currentUser);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityAdded_SetsCreatedAuditFields()
    {
        var user = Substitute.For<ICurrentUserService>();
        user.UserId.Returns("user-1");

        using var context = CreateContext(user);
        context.TestEntities.Add(new TestEntity());

        await context.SaveChangesAsync();

        var entity = context.TestEntities.First();
        Assert.Equal("user-1", entity.CreatedBy);
        Assert.True(entity.CreatedAtUtc > DateTime.MinValue);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityModified_SetsModifiedAuditFields()
    {
        var user = Substitute.For<ICurrentUserService>();
        user.UserId.Returns("user-1");

        using var context = CreateContext(user);
        var entity = new TestEntity();
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        entity.IsDeleted = false;
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();

        Assert.Equal("user-1", entity.ModifiedBy);
        Assert.NotNull(entity.ModifiedAtUtc);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityDeleted_AppliesSoftDelete()
    {
        var user = Substitute.For<ICurrentUserService>();
        user.UserId.Returns("admin");

        using var context = CreateContext(user);
        var entity = new TestEntity();
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        context.TestEntities.Remove(entity);
        await context.SaveChangesAsync();

        var saved = await context.TestEntities.IgnoreQueryFilters().FirstAsync();
        Assert.True(saved.IsDeleted);
        Assert.Equal("admin", saved.DeletedBy);
        Assert.NotNull(saved.DeletedAtUtc);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenNoCurrentUser_UsesSystemAsCreatedBy()
    {
        using var context = CreateContext(null);
        context.TestEntities.Add(new TestEntity());

        await context.SaveChangesAsync();

        var entity = context.TestEntities.First();
        Assert.Equal("system", entity.CreatedBy);
    }
}

public class UnitOfWorkTests
{
    private static TestAggregateContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestAggregateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TestAggregateContext(options);
    }

    [Fact]
    public async Task CompleteAsync_SavesChangesToDatabase()
    {
        using var context = CreateContext();
        var uow = new UnitOfWork<TestAggregateContext>(context, null,
            NullLogger<UnitOfWork<TestAggregateContext>>.Instance);

        context.Aggregates.Add(new TestAggregateRoot(Guid.NewGuid(), "TestAggregate"));
        var result = await uow.CompleteAsync();

        Assert.Equal(1, result);
        Assert.Single(context.Aggregates);
    }

    [Fact]
    public async Task CompleteAsync_WithNoDomainEventDispatcher_StillSaves()
    {
        using var context = CreateContext();
        var uow = new UnitOfWork<TestAggregateContext>(context);

        context.Aggregates.Add(new TestAggregateRoot(Guid.NewGuid(), "Test"));
        var result = await uow.CompleteAsync();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CompleteAsync_WithDomainEvents_DispatchesEvents()
    {
        using var context = CreateContext();
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var uow = new UnitOfWork<TestAggregateContext>(context, dispatcher,
            NullLogger<UnitOfWork<TestAggregateContext>>.Instance);

        var aggregate = new TestAggregateRoot(Guid.NewGuid(), "Test");
        aggregate.RaiseTestEvent();
        context.Aggregates.Add(aggregate);

        await uow.CompleteAsync();

        await dispatcher.Received(1).DispatchAsync(
            Arg.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteAsync_WithDomainEvents_ClearsDomainEventsAfterDispatch()
    {
        using var context = CreateContext();
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var uow = new UnitOfWork<TestAggregateContext>(context, dispatcher,
            NullLogger<UnitOfWork<TestAggregateContext>>.Instance);

        var aggregate = new TestAggregateRoot(Guid.NewGuid(), "Test");
        aggregate.RaiseTestEvent();
        context.Aggregates.Add(aggregate);

        await uow.CompleteAsync();

        Assert.Empty(aggregate.DomainEvents);
    }
}
