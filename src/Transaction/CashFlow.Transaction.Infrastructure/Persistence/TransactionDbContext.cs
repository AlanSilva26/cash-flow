using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transaction.Infrastructure.Persistence;

public sealed class TransactionDbContext(
    DbContextOptions<TransactionDbContext> options
) : DbContext(options),
    ITransactionPersistence
{
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();

    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
