using CashFlow.Transaction.Application;
using CashFlow.Transaction.Application.Abstractions.Messaging;
using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Application.FinancialTransactions.Create;
using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Transaction.UnitTests.Application.FinancialTransactions.Create;

public sealed class CreateFinancialTransactionValidationTests
{
    [Fact]
    public async Task Send_WhenCommandIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var repository = Substitute.For<IFinancialTransactionRepository>();
        var publisher = Substitute.For<IIntegrationEventPublisher>();
        var persistence = Substitute.For<ITransactionPersistence>();
        var sender = CreateSender(repository, publisher, persistence);

        var command = new CreateFinancialTransactionCommand(
            100.50m,
            TransactionType.Credit,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        await repository.Received(1)
                        .AddAsync(Arg.Any<FinancialTransaction>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.123)]
    public async Task Send_WhenAmountIsInvalid_ShouldReturnValidationFailure(decimal amount)
    {
        // Arrange
        var repository = Substitute.For<IFinancialTransactionRepository>();
        var sender = CreateSender(repository);

        var command = new CreateFinancialTransactionCommand(
            amount,
            TransactionType.Credit,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Failed");

        await repository.DidNotReceive()
                        .AddAsync(Arg.Any<FinancialTransaction>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_WhenTypeIsInvalid_ShouldReturnValidationFailure()
    {
        // Arrange
        var repository = Substitute.For<IFinancialTransactionRepository>();
        var sender = CreateSender(repository);

        var command = new CreateFinancialTransactionCommand(
            100m,
            (TransactionType)999,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Failed");

        await repository.DidNotReceive()
                        .AddAsync(Arg.Any<FinancialTransaction>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_WhenOccurredOnIsInTheFuture_ShouldReturnValidationFailure()
    {
        // Arrange
        var repository = Substitute.For<IFinancialTransactionRepository>();
        var sender = CreateSender(repository);

        var command = new CreateFinancialTransactionCommand(
            100m,
            TransactionType.Credit,
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
        );

        // Act
        var result = await sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Failed");

        await repository.DidNotReceive()
                        .AddAsync(Arg.Any<FinancialTransaction>(), Arg.Any<CancellationToken>());
    }

    private static ISender CreateSender(
        IFinancialTransactionRepository? repository = null,
        IIntegrationEventPublisher? publisher = null,
        ITransactionPersistence? persistence = null)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();
        services.AddSingleton(repository ?? Substitute.For<IFinancialTransactionRepository>());
        services.AddSingleton(publisher ?? Substitute.For<IIntegrationEventPublisher>());
        services.AddSingleton(persistence ?? Substitute.For<ITransactionPersistence>());

        return services.BuildServiceProvider().GetRequiredService<ISender>();
    }
}
