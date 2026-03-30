using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Api.Domain;
using Pokok.BuildingBlocks.Common;
using Pokok.BuildingBlocks.Persistence.EfCore;

namespace OrderManagement.Api.Infrastructure;

/// <summary>
/// DbContext for Order Management demonstrating Persistence building block
/// </summary>
public class OrderDbContext : DbContextBase
{
    public DbSet<Order> Orders { get; set; } = null!;

    public OrderDbContext(
        DbContextOptions<OrderDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options, currentUserService)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
            entity.Property(e => e.ModifiedAtUtc);

            // Configure domain events (not persisted)
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
