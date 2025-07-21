using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;

namespace Pokok.BuildingBlocks.Outbox;

public class OutboxDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .HasConversion(v => v.Value, v => OutboxMessageType.From(v))
                   .HasMaxLength(200);

            builder.Property(x => x.Payload)
                   .IsRequired();

            builder.Property(x => x.SourceApp)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.OccurredOnUtc);
            builder.Property(x => x.ProcessedOnUtc);
            builder.Property(x => x.Error)
                   .HasMaxLength(2000);
        });
    }
}