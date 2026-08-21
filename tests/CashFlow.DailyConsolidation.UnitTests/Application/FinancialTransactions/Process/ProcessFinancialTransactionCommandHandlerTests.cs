using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;
using NSubstitute;

namespace CashFlow.DailyConsolidation.UnitTests.Application.FinancialTransactions.Process;

public sealed class ProcessFinancialTransactionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTransactionIsCredit_ShouldApplyCreditAmount()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();

        var handler = new ProcessFinancialTransactionCommandHandler(repository);

        var command = new ProcessFinancialTransactionCommand(
            Id: Guid.NewGuid(),
            Amount: 100.50m,
            Type: "Credit",
            OccurredOn: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repository.Received(1)
                        .ApplyTransactionAsync(
                            command.Id,
                            command.OccurredOn,
                            command.Amount,
                            0m,
                            Arg.Any<CancellationToken>()
                        );
    }

    [Fact]
    public async Task Handle_WhenTransactionIsDebit_ShouldApplyDebitAmount()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();

        var handler = new ProcessFinancialTransactionCommandHandler(repository);

        var command = new ProcessFinancialTransactionCommand(
            Id: Guid.NewGuid(),
            Amount: 50.25m,
            Type: "Debit",
            OccurredOn: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repository.Received(1)
                        .ApplyTransactionAsync(
                            command.Id,
                            command.OccurredOn,
                            0m,
                            command.Amount,
                            Arg.Any<CancellationToken>()
                        );
    }
}
