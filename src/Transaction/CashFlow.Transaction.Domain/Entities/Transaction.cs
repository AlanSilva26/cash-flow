using CashFlow.Transaction.Domain.Common;
using CashFlow.Transaction.Domain.Enums;

namespace CashFlow.Transaction.Domain.Entities;

public sealed class Transaction
{
    public Guid Id { get; private set; }

    public decimal Amount { get; private set; }

    public TransactionType Type { get; private set; }

    public DateOnly OccurredOn { get; private set; }

    private Transaction() { }

    public Transaction(Guid id, decimal amount, TransactionType type, DateOnly occurredOn)
    {
        Id = Guard.AgainstEmpty(id, fieldName: "Id");
        Amount = ValidateAmount(amount);
        Type = Guard.AgainstInvalidEnum(type, fieldName: "Type");
        OccurredOn = Guard.AgainstFutureDate(occurredOn, fieldName: "Occurred On");
    }

    private static decimal ValidateAmount(decimal amount)
    {
        Guard.AgainstNonPositive(amount, fieldName: "Amount");

        return Guard.AgainstMoreThanTwoDecimalPlaces(amount, fieldName: "Amount");
    }
}
