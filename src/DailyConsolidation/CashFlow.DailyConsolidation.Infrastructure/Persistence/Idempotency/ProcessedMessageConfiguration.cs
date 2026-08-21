using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.DailyConsolidation.Infrastructure.Persistence.Idempotency;

internal sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("processed_messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Id)
               .ValueGeneratedNever();

        builder.Property(message => message.ProcessedOnUtc)
               .IsRequired();
    }
}
