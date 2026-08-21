using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;
using CashFlow.DailyConsolidation.Application.Models;
using FluentAssertions;
using NSubstitute;

namespace CashFlow.DailyConsolidation.UnitTests.Application.DailyBalances.GetByDate;

public sealed class GetDailyBalanceByDateQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenBalanceExists_ShouldReturnDailyBalance()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var expectedBalance = new DailyBalance
        {
            Date = date,
            TotalCredits = 150m,
            TotalDebits = 30m,
            Balance = 120m
        };

        repository.GetByDateAsync(date, Arg.Any<CancellationToken>())
                  .Returns(expectedBalance);

        var handler = new GetDailyBalanceByDateQueryHandler(repository);

        var query = new GetDailyBalanceByDateQuery(date);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedBalance);

        await repository.Received(1)
                        .GetByDateAsync(date, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBalanceDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        repository.GetByDateAsync(date, Arg.Any<CancellationToken>())
                  .Returns((DailyBalance?)null);

        var handler = new GetDailyBalanceByDateQueryHandler(repository);

        var query = new GetDailyBalanceByDateQuery(date);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DailyBalance.NotFound");
    }
}
