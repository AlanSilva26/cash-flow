using CashFlow.DailyConsolidation.Application.Models;
using MediatR;

namespace CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;

public sealed record GetDailyBalanceByDateQuery(DateOnly Date) : IRequest<DailyBalance?>;