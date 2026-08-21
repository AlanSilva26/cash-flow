using CashFlow.Shared.Contracts.Results;

namespace CashFlow.DailyConsolidation.Api.Extensions;

internal static class ResultExtensions
{
    public static IResult ToOk<TValue, TResponse>(
        this Result<TValue> result,
        Func<TValue, TResponse> map)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(map(result.Value));
        }

        return result.ToProblem();
    }

    private static IResult ToProblem(this Result result)
    {
        if (result.Error.Code.EndsWith(".NotFound", StringComparison.Ordinal))
        {
            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                [result.Error.Code] = [result.Error.Message]
            });
    }
}
