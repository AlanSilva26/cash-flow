using CashFlow.Transaction.Application.Abstractions.Messaging;
using CashFlow.Transaction.Application.Abstractions.Persistence;
using CashFlow.Transaction.Infrastructure.Messaging.RabbitMq;
using CashFlow.Transaction.Infrastructure.Persistence;
using CashFlow.Transaction.Infrastructure.Persistence.Outbox;
using CashFlow.Transaction.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Transaction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TransactionDatabase")
            ?? throw new InvalidOperationException("Connection string 'TransactionDatabase' was not found.");

        services.AddDbContext<TransactionDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<RabbitMqPublisher>();

        services.AddScoped<OutboxProcessor>();

        services.AddHostedService<OutboxBackgroundService>();

        services.AddScoped<IFinancialTransactionRepository, FinancialTransactionRepository>();

        services.AddScoped<ITransactionPersistence>(serviceProvider => serviceProvider.GetRequiredService<TransactionDbContext>());

        services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();

        return services;
    }
}
