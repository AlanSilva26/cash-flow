namespace CashFlow.DailyConsolidation.Infrastructure.Persistence.Idempotency;

internal sealed class ProcessedMessage
{
    public Guid Id { get; init; }

    public DateTime ProcessedOnUtc { get; init; }
}
