using CashFlow.Transaction.Domain.Enums;
using MediatR;

namespace CashFlow.Transaction.Application.FinancialTransactions.Create;

public sealed record CreateFinancialTransactionCommand(decimal Amount, TransactionType Type, DateOnly OccurredOn) : IRequest<Guid>;
