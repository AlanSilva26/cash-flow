namespace CashFlow.DailyConsolidation.Api.Endpoints;

internal sealed record DailyBalanceResponse(
    DateOnly Date,
    decimal TotalCredits,
    decimal TotalDebits,
    decimal Balance
);
