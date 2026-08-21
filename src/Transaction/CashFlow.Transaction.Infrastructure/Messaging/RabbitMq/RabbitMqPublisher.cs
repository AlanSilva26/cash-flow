using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CashFlow.Transaction.Infrastructure.Messaging.RabbitMq;

internal sealed class RabbitMqPublisher(IOptions<RabbitMqOptions> options)
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task PublishAsync(string messageType, string content, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            ),
            cancellationToken: cancellationToken
        );

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: _options.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: _options.DeadLetterQueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.DeadLetterRoutingKey,
            cancellationToken: cancellationToken
        );

        var arguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = _options.ExchangeName,
            ["x-dead-letter-routing-key"] = _options.DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: cancellationToken
        );

        var body = Encoding.UTF8.GetBytes(content);

        var properties = new BasicProperties
        {
            Persistent = true,
            Type = messageType
        };

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}
