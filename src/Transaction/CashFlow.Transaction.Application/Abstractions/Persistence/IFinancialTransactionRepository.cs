using CashFlow.Transaction.Domain.Entities;

namespace CashFlow.Transaction.Application.Abstractions.Persistence;

public interface IFinancialTransactionRepository
{
    Task AddAsync(FinancialTransaction financialTransaction, CancellationToken cancellationToken = default);
}
