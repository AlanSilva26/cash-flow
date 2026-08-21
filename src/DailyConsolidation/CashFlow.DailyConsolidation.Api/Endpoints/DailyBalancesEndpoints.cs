using CashFlow.DailyConsolidation.Api.Extensions;
using CashFlow.DailyConsolidation.Application.DailyBalances.GetByDate;
using MediatR;

namespace CashFlow.DailyConsolidation.Api.Endpoints;

internal static class DailyBalancesEndpoints
{
    public static IEndpointRouteBuilder MapDailyBalanceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/daily-balances")
                       .WithTags("Daily balances");

        group.MapGet("/{date}", async (
                DateOnly date,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetDailyBalanceByDateQuery(date);

                var result = await sender.Send(query, cancellationToken);

                return result.ToOk(balance => new DailyBalanceResponse(
                    balance.Date,
                    balance.TotalCredits,
                    balance.TotalDebits,
                    balance.Balance
                ));
            })
            .WithName("GetDailyBalanceByDate")
            .Produces<DailyBalanceResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
