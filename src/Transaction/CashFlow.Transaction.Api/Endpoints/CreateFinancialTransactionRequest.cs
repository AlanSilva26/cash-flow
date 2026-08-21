namespace CashFlow.Transaction.Api.Endpoints;

internal sealed record CreateFinancialTransactionRequest(decimal Amount, string Type, DateOnly OccurredOn);
