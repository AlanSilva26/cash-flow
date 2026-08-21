using CashFlow.Transaction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Transaction.Infrastructure.Persistence.Configurations;

internal sealed class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.ToTable("financial_transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Id)
               .ValueGeneratedNever();

        builder.Property(transaction => transaction.Amount)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(transaction => transaction.Type)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(transaction => transaction.OccurredOn)
               .IsRequired();
    }
}
