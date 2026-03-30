using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using Pokok.BuildingBlocks.Persistence.Converters;
using Pokok.BuildingBlocks.Persistence.Entities;
using Pokok.BuildingBlocks.Persistence.Extensions;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.EfCore;

internal class AuditableEntity : EntityBase { }

internal class ModelBuilderTestContext : DbContext
{
    public DbSet<AuditableEntity> Items => Set<AuditableEntity>();

    public ModelBuilderTestContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyGlobalConfigurations();
    }
}

public class ModelBuilderExtensionsTests
{
    [Fact]
    public async Task ApplyGlobalConfigurations_WithSoftDeletable_AddsQueryFilter()
    {
        var options = new DbContextOptionsBuilder<ModelBuilderTestContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ModelBuilderTestContext(options);

        var entity = new AuditableEntity();
        context.Items.Add(entity);
        await context.SaveChangesAsync();

        entity.IsDeleted = true;
        await context.SaveChangesAsync();

        var visible = await context.Items.ToListAsync();
        var all = await context.Items.IgnoreQueryFilters().ToListAsync();

        Assert.Empty(visible);
        Assert.Single(all);
    }

    [Fact]
    public async Task ApplyGlobalConfigurations_AddedEntity_CanBeRetrieved()
    {
        var options = new DbContextOptionsBuilder<ModelBuilderTestContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ModelBuilderTestContext(options);
        context.Items.Add(new AuditableEntity());
        await context.SaveChangesAsync();

        var all = context.Items.ToList();
        Assert.Single(all);
    }
}

public class UtcDateTimeConverterTests
{
    [Fact]
    public void ConvertToProvider_LocalDateTime_ConvertsToUtc()
    {
        var converter = new UtcDateTimeConverter();
        var local = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Local);

        var result = (DateTime)converter.ConvertToProvider(local)!;

        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void ConvertToProvider_UtcDateTime_ReturnsUtc()
    {
        var converter = new UtcDateTimeConverter();
        var utc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        var result = (DateTime)converter.ConvertToProvider(utc)!;

        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(utc, result);
    }

    [Fact]
    public void ConvertFromProvider_UnspecifiedDateTime_ConvertsToUtc()
    {
        var converter = new UtcDateTimeConverter();
        var stored = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Unspecified);

        var result = (DateTime)converter.ConvertFromProvider(stored)!;

        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }
}
