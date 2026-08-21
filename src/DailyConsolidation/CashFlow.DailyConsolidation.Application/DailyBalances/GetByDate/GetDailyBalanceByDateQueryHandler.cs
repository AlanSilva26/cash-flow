using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.Models;
using MediatR;

namespace CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;

internal sealed class GetDailyBalanceByDateQueryHandler(
    IDailyBalanceRepository dailyBalanceRepository
) : IRequestHandler<GetDailyBalanceByDateQuery, DailyBalance?>
{
    public async Task<DailyBalance?> Handle(GetDailyBalanceByDateQuery request, CancellationToken cancellationToken)
    {
        return await dailyBalanceRepository.GetByDateAsync(request.Date, cancellationToken);
    }
}