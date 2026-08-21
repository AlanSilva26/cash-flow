namespace CashFlow.DailyConsolidation.Application.Models;

public sealed class DailyBalance
{
    public DateOnly Date { get; init; }

    public decimal TotalCredits { get; set; }

    public decimal TotalDebits { get; set; }

    public decimal Balance { get; set; }
}