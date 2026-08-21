using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using MediatR;

namespace CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;

internal sealed class ProcessFinancialTransactionCommandHandler(
    IDailyBalanceRepository dailyBalanceRepository
) : IRequestHandler<ProcessFinancialTransactionCommand>
{
    public async Task Handle(ProcessFinancialTransactionCommand request, CancellationToken cancellationToken)
    {
        var creditAmount = request.Type == "Credit"
            ? request.Amount
            : 0m;

        var debitAmount = request.Type == "Debit"
            ? request.Amount
            : 0m;

        await dailyBalanceRepository.ApplyTransactionAsync(request.OccurredOn, creditAmount, debitAmount, cancellationToken);
    }
}