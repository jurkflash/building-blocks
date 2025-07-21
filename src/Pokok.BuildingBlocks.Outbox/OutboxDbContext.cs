using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        var outboxTypeConverter = new ValueConverter<OutboxMessageType, string>(
            v => v.Value,
            v => OutboxMessageType.From(v)
        );

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .HasConversion(outboxTypeConverter) 
                   .HasMaxLength(200)
                   .IsRequired();

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