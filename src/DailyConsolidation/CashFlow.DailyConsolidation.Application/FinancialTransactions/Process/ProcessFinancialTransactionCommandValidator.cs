using FluentValidation;

namespace CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;

internal sealed class ProcessFinancialTransactionCommandValidator : AbstractValidator<ProcessFinancialTransactionCommand>
{
    private static readonly string[] ValidTypes = ["Credit", "Debit"];

    public ProcessFinancialTransactionCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        RuleFor(command => command.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(command => command.Type)
            .Must(type => ValidTypes.Contains(type))
            .WithMessage("Type is invalid.");

        RuleFor(command => command.OccurredOn)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("OccurredOn cannot be in the future.");
    }
}
