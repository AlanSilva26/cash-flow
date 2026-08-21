using CashFlow.DailyConsolidation.Infrastructure.Persistence;
using CashFlow.DailyConsolidation.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.DailyConsolidation.IntegrationTests.Persistence;

public sealed class DailyBalancePersistenceTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=cash_flow_daily_consolidation;Username=postgres;Password=postgres";

    [Fact]
    public async Task ApplyTransactionAsync_WhenTransactionsHaveSameDate_ShouldAccumulateDailyBalance()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DailyConsolidationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var firstTransactionId = Guid.NewGuid();
        var secondTransactionId = Guid.NewGuid();
        var thirdTransactionId = Guid.NewGuid();

        var transactionIds = new[]
        {
            firstTransactionId,
            secondTransactionId,
            thirdTransactionId
        };

        await using var context = new DailyConsolidationDbContext(options);

        await context.DailyBalances
                     .Where(balance => balance.Date == date)
                     .ExecuteDeleteAsync();

        var repository = new DailyBalanceRepository(context);

        try
        {
            // Act
            await repository.ApplyTransactionAsync(
                firstTransactionId,
                date,
                creditAmount: 100m,
                debitAmount: 0m);

            await repository.ApplyTransactionAsync(
                secondTransactionId,
                date,
                creditAmount: 50m,
                debitAmount: 0m);

            await repository.ApplyTransactionAsync(
                thirdTransactionId,
                date,
                creditAmount: 0m,
                debitAmount: 30m);

            // Assert
            var dailyBalance = await repository.GetByDateAsync(date);

            dailyBalance.Should().NotBeNull();

            dailyBalance!.TotalCredits.Should().Be(150m);
            dailyBalance.TotalDebits.Should().Be(30m);
            dailyBalance.Balance.Should().Be(120m);
        }
        finally
        {
            await context.DailyBalances
                         .Where(balance => balance.Date == date)
                         .ExecuteDeleteAsync();

            await context.ProcessedMessages
                         .Where(message => transactionIds.Contains(message.Id))
                         .ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task ApplyTransactionAsync_WhenSameTransactionIsProcessedTwice_ShouldApplyBalanceOnlyOnce()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DailyConsolidationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var transactionId = Guid.NewGuid();

        await using var context = new DailyConsolidationDbContext(options);

        await context.DailyBalances
                     .Where(balance => balance.Date == date)
                     .ExecuteDeleteAsync();

        await context.ProcessedMessages
                     .Where(message => message.Id == transactionId)
                     .ExecuteDeleteAsync();

        var repository = new DailyBalanceRepository(context);

        try
        {
            // Act
            await repository.ApplyTransactionAsync(
                transactionId,
                date,
                creditAmount: 100m,
                debitAmount: 0m);

            await repository.ApplyTransactionAsync(
                transactionId,
                date,
                creditAmount: 100m,
                debitAmount: 0m);

            // Assert
            var dailyBalance = await repository.GetByDateAsync(date);

            dailyBalance.Should().NotBeNull();

            dailyBalance!.TotalCredits.Should().Be(100m);
            dailyBalance.TotalDebits.Should().Be(0m);
            dailyBalance.Balance.Should().Be(100m);

            var processedMessageCount = await context.ProcessedMessages
                .AsNoTracking()
                .CountAsync(message => message.Id == transactionId);

            processedMessageCount.Should().Be(1);
        }
        finally
        {
            await context.DailyBalances
                         .Where(balance => balance.Date == date)
                         .ExecuteDeleteAsync();

            await context.ProcessedMessages
                         .Where(message => message.Id == transactionId)
                         .ExecuteDeleteAsync();
        }
    }
}
