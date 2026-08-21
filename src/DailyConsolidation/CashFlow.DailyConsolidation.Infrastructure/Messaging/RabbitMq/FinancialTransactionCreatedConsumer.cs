using CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;
using CashFlow.Shared.Contracts.FinancialTransactions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CashFlow.DailyConsolidation.Infrastructure.Messaging.RabbitMq;

internal sealed class FinancialTransactionCreatedConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<FinancialTransactionCreatedConsumer> logger
) : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.QueueDeclareAsync(
            queue: _options.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.QueueBindAsync(
            queue: _options.DeadLetterQueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.DeadLetterRoutingKey,
            cancellationToken: stoppingToken
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
            cancellationToken: stoppingToken
        );

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var content = Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray()
                );

                var integrationEvent = JsonSerializer.Deserialize<FinancialTransactionCreatedIntegrationEvent>(content);

                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("Could not deserialize financial transaction event.");
                }

                await using var scope = scopeFactory.CreateAsyncScope();

                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var command = new ProcessFinancialTransactionCommand(
                    integrationEvent.Id,
                    integrationEvent.Amount,
                    integrationEvent.Type,
                    integrationEvent.OccurredOn
                );

                await sender.Send(
                    command,
                    stoppingToken
                );

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "An error occurred while processing a financial transaction event."
                );

                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken
                );
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken
        );
    }
}