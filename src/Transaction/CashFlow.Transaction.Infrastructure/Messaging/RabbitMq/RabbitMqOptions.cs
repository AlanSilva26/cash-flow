namespace CashFlow.Transaction.Infrastructure.Messaging.RabbitMq;

internal sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = string.Empty;

    public int Port { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string ExchangeName { get; init; } = string.Empty;

    public string RoutingKey { get; init; } = string.Empty;
}
