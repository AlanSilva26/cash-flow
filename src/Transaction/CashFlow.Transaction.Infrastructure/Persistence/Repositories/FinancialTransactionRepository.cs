using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Domain.Entities;

namespace CashFlow.Transaction.Infrastructure.Persistence.Repositories;

internal sealed class FinancialTransactionRepository(TransactionDbContext context) : IFinancialTransactionRepository
{
    public async Task AddAsync(FinancialTransaction financialTransaction, CancellationToken cancellationToken = default)
    {
        await context.FinancialTransactions.AddAsync(financialTransaction, cancellationToken);
    }
}
