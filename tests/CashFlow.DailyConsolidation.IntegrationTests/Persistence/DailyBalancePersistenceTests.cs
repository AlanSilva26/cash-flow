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

        await using var context = new DailyConsolidationDbContext(options);

        await context.DailyBalances.Where(balance => balance.Date == date)
                                   .ExecuteDeleteAsync();

        var repository = new DailyBalanceRepository(context);

        try
        {
            // Act
            await repository.ApplyTransactionAsync(date, creditAmount: 100m, debitAmount: 0m);

            await repository.ApplyTransactionAsync(date, creditAmount: 50m, debitAmount: 0m);

            await repository.ApplyTransactionAsync(date, creditAmount: 0m, debitAmount: 30m);

            // Assert
            var dailyBalance = await repository.GetByDateAsync(date);

            dailyBalance.Should().NotBeNull();

            dailyBalance!.TotalCredits.Should().Be(150m);
            dailyBalance.TotalDebits.Should().Be(30m);
            dailyBalance.Balance.Should().Be(120m);
        }
        finally
        {
            await context.DailyBalances.Where(balance => balance.Date == date)
                                       .ExecuteDeleteAsync();
        }
    }
}