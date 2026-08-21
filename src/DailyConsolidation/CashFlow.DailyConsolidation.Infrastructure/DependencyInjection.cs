using CashFlow.DailyConsolidation.Application.Abstractions.Persistence;
using CashFlow.DailyConsolidation.Infrastructure.Persistence;
using CashFlow.DailyConsolidation.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.DailyConsolidation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DailyConsolidationDatabase")
            ?? throw new InvalidOperationException("Connection string 'DailyConsolidationDatabase' was not found.");

        services.AddDbContext<DailyConsolidationDbContext>(
            options => options.UseNpgsql(connectionString)
        );

        services.AddScoped<IDailyBalanceRepository, DailyBalanceRepository>();

        return services;
    }
}