namespace CashFlow.Transaction.Application.Abstractions.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default);
}
