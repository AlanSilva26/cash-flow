using CashFlow.DailyConsolidation.Application.Models;

namespace CashFlow.DailyConsolidation.Application.Abstractions.Persistence;

public interface IDailyBalanceRepository
{
    Task<DailyBalance?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task ApplyTransactionAsync(DateOnly date, decimal creditAmount, decimal debitAmount, CancellationToken cancellationToken = default);
}