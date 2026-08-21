using CashFlow.Shared.Contracts.FinancialTransactions;
using CashFlow.Transaction.Application.Abstractions.Messaging;
using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Application.FinancialTransactions.Create;
using CashFlow.Transaction.Domain.Entities;
using CashFlow.Transaction.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace CashFlow.Transaction.UnitTests.Application.FinancialTransactions.Create;

public sealed class CreateFinancialTransactionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldPersistTransactionAndReturnId()
    {
        // Arrange
        var repository = Substitute.For<IFinancialTransactionRepository>();
        var publisher = Substitute.For<IIntegrationEventPublisher>();
        var persistence = Substitute.For<ITransactionPersistence>();

        var handler = new CreateFinancialTransactionCommandHandler(repository, publisher, persistence);

        var command = new CreateFinancialTransactionCommand(100.50m, TransactionType.Credit, DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        id.Should().NotBeEmpty();

        await repository.Received(1)
                        .AddAsync(
                            Arg.Is<FinancialTransaction>(transaction =>
                                transaction.Id == id &&
                                transaction.Amount == command.Amount &&
                                transaction.Type == command.Type &&
                                transaction.OccurredOn == command.OccurredOn),
                            Arg.Any<CancellationToken>()
                        );

        await publisher.Received(1)
                       .PublishAsync(Arg.Is<FinancialTransactionCreatedIntegrationEvent>(integrationEvent =>
                           integrationEvent.Id == id
                           && integrationEvent.Amount == command.Amount
                           && integrationEvent.Type == command.Type.ToString()
                           && integrationEvent.OccurredOn == command.OccurredOn
                       ),
                       Arg.Any<CancellationToken>()
        );

        await persistence.Received(1)
                         .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
