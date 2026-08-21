using CashFlow.Transaction.Infrastructure.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transaction.Infrastructure.Persistence.Outbox;

internal sealed class OutboxProcessor(TransactionDbContext context, RabbitMqPublisher rabbitMqPublisher)
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await context.OutboxMessages.Where(message => message.ProcessedOnUtc == null)
                                                   .OrderBy(message => message.OccurredOnUtc)
                                                   .Take(100)
                                                   .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await rabbitMqPublisher.PublishAsync(message.Type, message.Content, cancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception exception)
            {
                message.Error = exception.Message;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
