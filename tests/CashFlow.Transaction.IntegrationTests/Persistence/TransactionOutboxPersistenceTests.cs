using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Domain.Enums;
using CashFlow.Transaction.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transaction.IntegrationTests.Persistence;

public sealed class TransactionOutboxPersistenceTests
{
    //private const string ConnectionString = "Host=localhost;Port=5432;Database=cash_flow_transaction;Username=postgres;Password=postgres";

    //[Fact]
    //public async Task SaveChangesAsync_WhenTransactionAndOutboxMessageAreAdded_ShouldPersistBoth()
    //{
    //    // Arrange
    //    var options = new DbContextOptionsBuilder<TransactionDbContext>()
    //        .UseNpgsql(ConnectionString)
    //        .Options;

    //    var financialTransaction = new FinancialTransaction(
    //        id: Guid.NewGuid(),
    //        amount: 100.50m,
    //        type: TransactionType.Credit,
    //        occurredOn: DateOnly.FromDateTime(DateTime.UtcNow)
    //    );

    //    await using var context = new TransactionDbContext(options);

    //    // Act
    //    await context.FinancialTransactions.AddAsync(financialTransaction);

    //    await context.Database.ExecuteSqlInterpolatedAsync(
    //        $"""
    //        INSERT INTO outbox_messages
    //            ("Id", "Type", "Content", "OccurredOnUtc")
    //        VALUES
    //            ({Guid.NewGuid()}, {"FinancialTransactionCreated"}, {"{}"}, {DateTime.UtcNow});
    //        """
    //    );

    //    await context.SaveChangesAsync();

    //    // Assert
    //    var transactionExists = await context.FinancialTransactions.AsNoTracking()
    //                                                               .AnyAsync(transaction => transaction.Id == financialTransaction.Id);

    //    transactionExists.Should().BeTrue();
    //}
}