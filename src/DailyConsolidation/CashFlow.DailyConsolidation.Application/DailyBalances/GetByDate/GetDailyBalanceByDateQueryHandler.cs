using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Application.Models;
using CashFlow.Shared.Contracts.Results;
using MediatR;

namespace CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;

internal sealed class GetDailyBalanceByDateQueryHandler(
    IDailyBalanceRepository dailyBalanceRepository
) : IRequestHandler<GetDailyBalanceByDateQuery, Result<DailyBalance>>
{
    public async Task<Result<DailyBalance>> Handle(GetDailyBalanceByDateQuery request, CancellationToken cancellationToken)
    {
        var dailyBalance = await dailyBalanceRepository.GetByDateAsync(request.Date, cancellationToken);

        if (dailyBalance is null)
        {
            return Result<DailyBalance>.Failure(
                Error.NotFound("DailyBalance.NotFound", "Daily balance was not found for the requested date.")
            );
        }

        return Result<DailyBalance>.Success(dailyBalance);
    }
}
