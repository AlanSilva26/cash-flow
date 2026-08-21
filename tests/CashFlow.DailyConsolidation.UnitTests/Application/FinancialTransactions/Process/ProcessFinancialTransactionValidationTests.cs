using CashFlow.DailyConsolidation.Application;
using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.DailyConsolidation.UnitTests.Application.FinancialTransactions.Process;

public sealed class ProcessFinancialTransactionValidationTests
{
    [Fact]
    public async Task Send_WhenCommandIsValid_ShouldProcessTransaction()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();
        var sender = CreateSender(repository);

        var command = new ProcessFinancialTransactionCommand(
            Guid.NewGuid(),
            100m,
            "Credit",
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        await sender.Send(command);

        // Assert
        await repository.Received(1)
                        .ApplyTransactionAsync(
                            command.Id,
                            command.OccurredOn,
                            command.Amount,
                            0m,
                            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_WhenCommandIsInvalid_ShouldThrowValidationException()
    {
        // Arrange
        var repository = Substitute.For<IDailyBalanceRepository>();
        var sender = CreateSender(repository);

        var command = new ProcessFinancialTransactionCommand(
            Guid.Empty,
            0m,
            "Invalid",
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        );

        // Act
        var act = () => sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();

        await repository.DidNotReceive()
                        .ApplyTransactionAsync(
                            Arg.Any<Guid>(),
                            Arg.Any<DateOnly>(),
                            Arg.Any<decimal>(),
                            Arg.Any<decimal>(),
                            Arg.Any<CancellationToken>());
    }

    private static ISender CreateSender(IDailyBalanceRepository repository)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();
        services.AddSingleton(repository);

        return services.BuildServiceProvider().GetRequiredService<ISender>();
    }
}
