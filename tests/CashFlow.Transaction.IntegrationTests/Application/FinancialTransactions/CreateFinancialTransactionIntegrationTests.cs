using CashFlow.Transaction.Application;
using CashFlow.Transaction.Application.FinancialTransactions.Create;
using CashFlow.Transaction.Domain.Enums;
using CashFlow.Transaction.Infrastructure;
using CashFlow.Transaction.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Transaction.IntegrationTests.Application.FinancialTransactions;

public sealed class CreateFinancialTransactionIntegrationTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=cash_flow_transaction;Username=postgres;Password=postgres";

    [Fact]
    public async Task Send_WhenCommandIsValid_ShouldPersistTransactionAndOutboxMessage()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TransactionDatabase"] = ConnectionString
            })
            .Build();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);

        await using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();

        var command = new CreateFinancialTransactionCommand(
            Amount: 100.50m,
            Type: TransactionType.Credit,
            OccurredOn: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var transactionId = Guid.Empty;

        try
        {
            // Act
            transactionId = await sender.Send(command);

            // Assert
            var financialTransaction = await context.FinancialTransactions.AsNoTracking()
                                                                          .SingleOrDefaultAsync(transaction => transaction.Id == transactionId);

            financialTransaction.Should().NotBeNull();

            financialTransaction!.Amount.Should().Be(command.Amount);
            financialTransaction.Type.Should().Be(command.Type);
            financialTransaction.OccurredOn.Should().Be(command.OccurredOn);

            var outboxMessageCount = await context.Database
                .SqlQuery<int>(
                    $"""
                    SELECT COUNT(*)::int AS "Value"
                    FROM outbox_messages
                    WHERE "Content" ->> 'Id' = {transactionId.ToString()}
                    """)
                .SingleAsync();

            outboxMessageCount.Should().Be(1);
        }
        finally
        {
            if (transactionId != Guid.Empty)
            {
                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                    DELETE FROM outbox_messages
                    WHERE "Content" ->> 'Id' = {transactionId.ToString()};
                    """);

                await context.FinancialTransactions.Where(transaction => transaction.Id == transactionId)
                                                   .ExecuteDeleteAsync();
            }
        }
    }
}