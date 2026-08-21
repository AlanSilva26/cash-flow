using CashFlow.Transaction.Application.Abstractions.Messaging;
using System.Text.Json;

namespace CashFlow.Transaction.Infrastructure.Persistence.Outbox;

internal sealed class OutboxIntegrationEventPublisher(TransactionDbContext context) : IIntegrationEventPublisher
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).Name,
            Content = JsonSerializer.Serialize(integrationEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
