namespace CashFlow.Shared.Contracts.FinancialTransactions;

public sealed record FinancialTransactionCreatedIntegrationEvent(Guid Id, decimal Amount, string Type, DateOnly OccurredOn);
