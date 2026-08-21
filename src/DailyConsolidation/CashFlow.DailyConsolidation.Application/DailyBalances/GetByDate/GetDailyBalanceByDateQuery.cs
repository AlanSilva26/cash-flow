using CashFlow.DailyConsolidation.Application.Models;
using CashFlow.Shared.Contracts.Results;
using MediatR;

namespace CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;

public sealed record GetDailyBalanceByDateQuery(DateOnly Date) : IRequest<Result<DailyBalance>>;
