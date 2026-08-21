using FluentValidation;

namespace CashFlow.Transaction.Application.FinancialTransactions.Create;

internal sealed class CreateFinancialTransactionCommandValidator : AbstractValidator<CreateFinancialTransactionCommand>
{
    public CreateFinancialTransactionCommandValidator()
    {
        RuleFor(command => command.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Amount cannot have more than two decimal places.");

        RuleFor(command => command.Type)
            .Must(type => Enum.IsDefined(type))
            .WithMessage("Type is invalid.");

        RuleFor(command => command.OccurredOn)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("OccurredOn cannot be in the future.");
    }

    private static bool HaveNoMoreThanTwoDecimalPlaces(decimal amount)
    {
        return decimal.Round(amount, 2) == amount;
    }
}
