using CashFlow.Shared.Contracts.FinancialTransactions;
using CashFlow.Transaction.Application.Abstractions.Messaging;
using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Domain.Entities;
using MediatR;

namespace CashFlow.Transaction.Application.FinancialTransactions.Create;

internal sealed class CreateFinancialTransactionCommandHandler(
    IFinancialTransactionRepository financialTransactionRepository,
    IIntegrationEventPublisher integrationEventPublisher,
    ITransactionPersistence transactionPersistence
) : IRequestHandler<CreateFinancialTransactionCommand, Guid>
{
    public async Task<Guid> Handle(CreateFinancialTransactionCommand request, CancellationToken cancellationToken)
    {
        var financialTransaction = new FinancialTransaction(
            Guid.NewGuid(),
            request.Amount,
            request.Type,
            request.OccurredOn
        );

        await financialTransactionRepository.AddAsync(financialTransaction, cancellationToken);

        var integrationEvent = new FinancialTransactionCreatedIntegrationEvent(
            financialTransaction.Id,
            financialTransaction.Amount,
            financialTransaction.Type.ToString(),
            financialTransaction.OccurredOn
        );

        await integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);

        await transactionPersistence.SaveChangesAsync(cancellationToken);

        return financialTransaction.Id;
    }
}
