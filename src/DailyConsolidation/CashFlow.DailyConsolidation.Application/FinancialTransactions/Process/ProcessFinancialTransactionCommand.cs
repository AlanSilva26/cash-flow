using MediatR;

namespace CashFlow.DailyConsolidation.Application.FinancialTransactions.Process;

public sealed record ProcessFinancialTransactionCommand(Guid Id, decimal Amount, string Type, DateOnly OccurredOn) : IRequest;