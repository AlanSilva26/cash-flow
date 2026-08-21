using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.DailyConsolidation.Infrastructure.Persistence.Repositories;

internal sealed class DailyBalanceRepository(DailyConsolidationDbContext context) : IDailyBalanceRepository
{
    public async Task ApplyTransactionAsync(Guid transactionId, DateOnly date, decimal creditAmount, decimal debitAmount, CancellationToken cancellationToken = default)
    {
        var processedOnUtc = DateTime.UtcNow;

        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            WITH processed_message AS
            (
                INSERT INTO processed_messages
                    ("Id", "ProcessedOnUtc")
                VALUES
                    ({transactionId}, {processedOnUtc})
                ON CONFLICT ("Id") DO NOTHING
                RETURNING "Id"
            )
            INSERT INTO daily_balances
                ("Date", "TotalCredits", "TotalDebits", "Balance")
            SELECT
                {date},
                {creditAmount},
                {debitAmount},
                {creditAmount - debitAmount}
            FROM processed_message
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