using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transaction.Infrastructure.Persistence.Outbox;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Id)
               .ValueGeneratedNever();

        builder.Property(message => message.Type)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(message => message.Content)
               .HasColumnType("jsonb")
               .IsRequired();

        builder.Property(message => message.OccurredOnUtc)
               .IsRequired();

        builder.Property(message => message.ProcessedOnUtc);

        builder.Property(message => message.Error);

        builder.HasIndex(message => message.ProcessedOnUtc);
    }
}
