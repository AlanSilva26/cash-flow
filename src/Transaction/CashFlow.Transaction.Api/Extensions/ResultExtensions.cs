using CashFlow.Shared.Contracts.Results;

namespace CashFlow.Transaction.Api.Extensions;

internal static class ResultExtensions
{
    public static IResult ToCreated<TValue, TResponse>(
        this Result<TValue> result,
        Func<TValue, TResponse> map)
    {
        if (result.IsSuccess)
        {
            return Results.Json(map(result.Value), statusCode: StatusCodes.Status201Created);
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
