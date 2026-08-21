using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Domain.Enums;
using CashFlow.Transaction.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transaction.IntegrationTests.Persistence;

public sealed class FinancialTransactionPersistenceTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=cash_flow_transaction;Username=postgres;Password=postgres";

    [Fact]
    public async Task SaveChangesAsync_WhenFinancialTransactionIsAdded_ShouldPersistTransaction()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var financialTransaction = new FinancialTransaction(
            id: Guid.NewGuid(),
            amount: 100.50m,
            type: TransactionType.Credit,
            occurredOn: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await using var context = new TransactionDbContext(options);

        // Act
        await context.FinancialTransactions.AddAsync(financialTransaction);
        await context.SaveChangesAsync();

        // Assert
        var persistedTransaction = await context.FinancialTransactions.AsNoTracking()
                                                                      .SingleOrDefaultAsync(transaction => transaction.Id == financialTransaction.Id);

        persistedTransaction.Should().NotBeNull();
        persistedTransaction!.Amount.Should().Be(financialTransaction.Amount);
        persistedTransaction.Type.Should().Be(financialTransaction.Type);
        persistedTransaction.OccurredOn.Should().Be(financialTransaction.OccurredOn);
    }
}