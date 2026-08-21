using CashFlow.Transaction.Api.Extensions;
using CashFlow.Transaction.Application.FinancialTransactions.Create;
using CashFlow.Transaction.Domain.Enums;
using MediatR;

namespace CashFlow.Transaction.Api.Endpoints;

internal static class TransactionsEndpoints
{
    public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions")
                       .WithTags("Transactions");

        group.MapPost("/", async (
                CreateFinancialTransactionRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var type = Enum.TryParse<TransactionType>(request.Type, ignoreCase: true, out var parsedType)
                    ? parsedType
                    : default;

                var command = new CreateFinancialTransactionCommand(
                    request.Amount,
                    type,
                    request.OccurredOn
                );

                var result = await sender.Send(command, cancellationToken);

                return result.ToCreated(id => new CreateFinancialTransactionResponse(id));
            })
            .WithName("CreateFinancialTransaction")
            .Produces<CreateFinancialTransactionResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
