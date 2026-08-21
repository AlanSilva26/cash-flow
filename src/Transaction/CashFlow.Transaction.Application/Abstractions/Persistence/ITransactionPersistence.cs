namespace CashFlow.Transaction.Application.Abstractions.Persistence;

public interface ITransactionPersistence
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
