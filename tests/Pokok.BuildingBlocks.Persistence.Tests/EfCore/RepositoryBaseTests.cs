using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Base;
using Pokok.BuildingBlocks.Persistence.Entities;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.EfCore;

internal class WidgetEntity : EntityBase
{
    public string Name { get; set; } = string.Empty;
}

internal class WidgetContext : DbContext
{
    public DbSet<WidgetEntity> Widgets => Set<WidgetEntity>();

    public WidgetContext(DbContextOptions options) : base(options) { }
}

internal class WidgetRepository : RepositoryBase<WidgetEntity, WidgetContext>
{
    public WidgetRepository(WidgetContext context) : base(context) { }
}

public class RepositoryBaseTests
{
    private static WidgetContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WidgetContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new WidgetContext(options);
    }

    [Fact]
    public async Task AddAsync_WithEntity_PersistsToDatabase()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        var widget = new WidgetEntity { Name = "Widget1" };

        await repo.AddAsync(widget);
        await context.SaveChangesAsync();

        Assert.Single(context.Widgets);
    }

    [Fact]
    public async Task GetAsync_WithExistingId_ReturnsEntity()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        var widget = new WidgetEntity { Name = "Widget1" };
        await repo.AddAsync(widget);
        await context.SaveChangesAsync();

        var found = await repo.GetAsync(widget.Id);

        Assert.NotNull(found);
        Assert.Equal("Widget1", found.Name);
    }

    [Fact]
    public async Task GetAsync_WithNonExistingId_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);

        var found = await repo.GetAsync(Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleEntities_ReturnsAll()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        await repo.AddRangeAsync(new[] { new WidgetEntity { Name = "A" }, new WidgetEntity { Name = "B" } });
        await context.SaveChangesAsync();

        var all = await repo.GetAllAsync();

        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ReturnsMatchingEntities()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        await repo.AddRangeAsync(new[] { new WidgetEntity { Name = "Alpha" }, new WidgetEntity { Name = "Beta" } });
        await context.SaveChangesAsync();

        var result = await repo.FindAsync(w => w.Name.StartsWith("Al"));

        Assert.Single(result);
        Assert.Equal("Alpha", result.First().Name);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithMatchingPredicate_ReturnsEntity()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        var widget = new WidgetEntity { Name = "Unique" };
        await repo.AddAsync(widget);
        await context.SaveChangesAsync();

        var result = await repo.SingleOrDefaultAsync(w => w.Name == "Unique");

        Assert.NotNull(result);
        Assert.Equal("Unique", result.Name);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithNonMatchingPredicate_ReturnsNull()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);

        var result = await repo.SingleOrDefaultAsync(w => w.Name == "NotExist");

        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_WithEntity_DeletesFromDatabase()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        var widget = new WidgetEntity { Name = "ToDelete" };
        await repo.AddAsync(widget);
        await context.SaveChangesAsync();

        repo.Remove(widget);
        await context.SaveChangesAsync();

        Assert.Empty(context.Widgets);
    }

    [Fact]
    public async Task RemoveRange_WithEntities_DeletesAll()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        var widgets = new[] { new WidgetEntity { Name = "A" }, new WidgetEntity { Name = "B" } };
        await repo.AddRangeAsync(widgets);
        await context.SaveChangesAsync();

        repo.RemoveRange(widgets);
        await context.SaveChangesAsync();

        Assert.Empty(context.Widgets);
    }

    [Fact]
    public async Task ExistsAsync_WithMatchingEntity_ReturnsTrue()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        await repo.AddAsync(new WidgetEntity { Name = "Present" });
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(w => w.Name == "Present", CancellationToken.None);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithNoMatchingEntity_ReturnsFalse()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);

        var exists = await repo.ExistsAsync(w => w.Name == "Ghost", CancellationToken.None);

        Assert.False(exists);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
    {
        using var context = CreateContext();
        var repo = new WidgetRepository(context);
        await repo.AddRangeAsync(new[] { new WidgetEntity { Name = "Alpha" }, new WidgetEntity { Name = "Beta" }, new WidgetEntity { Name = "Gamma" } });
        await context.SaveChangesAsync();

        var count = await repo.CountAsync(w => w.Name.StartsWith("A") || w.Name.StartsWith("B"), CancellationToken.None);

        Assert.Equal(2, count);
    }
}
