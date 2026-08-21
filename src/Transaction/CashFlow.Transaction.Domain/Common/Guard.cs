using CashFlow.Transaction.Domain.Exceptions;

namespace CashFlow.Transaction.Domain.Common;

internal static class Guard
{
    internal static Guid AgainstEmpty(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
            throw new DomainException($"{fieldName} cannot be empty.");

        return value;
    }

    internal static decimal AgainstNonPositive(decimal value, string fieldName)
    {
        if (value <= 0)
            throw new DomainException($"{fieldName} must be greater than zero.");

        return value;
    }

    internal static TEnum AgainstInvalidEnum<TEnum>(TEnum value, string fieldName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new DomainException($"{fieldName} is invalid.");

        return value;
    }

    internal static DateOnly AgainstFutureDate(DateOnly value, string fieldName)
    {
        if (value > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException($"{fieldName} cannot be in the future.");

        return value;
    }

    internal static decimal AgainstMoreThanTwoDecimalPlaces(decimal value, string fieldName)
    {
        if (decimal.Round(value, 2) != value)
            throw new DomainException($"{fieldName} cannot have more than two decimal places.");

        return value;
    }
}
