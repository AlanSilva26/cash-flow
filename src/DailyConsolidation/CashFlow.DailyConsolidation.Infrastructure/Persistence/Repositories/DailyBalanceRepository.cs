using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.DailyConsolidation.Infrastructure.Persistence.Repositories;

internal sealed class DailyBalanceRepository(DailyConsolidationDbContext context) : IDailyBalanceRepository
{
    public async Task ApplyTransactionAsync(DateOnly date, decimal creditAmount, decimal debitAmount, CancellationToken cancellationToken = default)
    {
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO daily_balances
                ("Date", "TotalCredits", "TotalDebits", "Balance")
            VALUES
                ({date}, {creditAmount}, {debitAmount}, {creditAmount - debitAmount})
            ON CONFLICT ("Date")
            DO UPDATE SET
                "TotalCredits" = daily_balances."TotalCredits" + EXCLUDED."TotalCredits",
                "TotalDebits" = daily_balances."TotalDebits" + EXCLUDED."TotalDebits",
                "Balance" = daily_balances."Balance" + EXCLUDED."Balance";
            """,
            cancellationToken);
    }

    public async Task<DailyBalance?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await context.DailyBalances.AsNoTracking()
                                          .SingleOrDefaultAsync(balance => balance.Date == date, cancellationToken);
    }
}