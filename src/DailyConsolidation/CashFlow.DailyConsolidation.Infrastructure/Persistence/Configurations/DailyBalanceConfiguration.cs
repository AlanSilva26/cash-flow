using CashFlow.DailyConsolidation.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.DailyConsolidation.Infrastructure.Persistence.Configurations;

internal sealed class DailyBalanceConfiguration : IEntityTypeConfiguration<DailyBalance>
{
    public void Configure(EntityTypeBuilder<DailyBalance> builder)
    {
        builder.ToTable("daily_balances");

        builder.HasKey(balance => balance.Date);

        builder.Property(balance => balance.Date)
               .ValueGeneratedNever();

        builder.Property(balance => balance.TotalCredits)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(balance => balance.TotalDebits)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(balance => balance.Balance)
               .HasPrecision(18, 2)
               .IsRequired();
    }
}