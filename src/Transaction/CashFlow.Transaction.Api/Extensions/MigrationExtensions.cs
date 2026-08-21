using CashFlow.Transaction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Transaction.Api.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("ApplyMigrations"))
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();

        await context.Database.MigrateAsync();
    }
}
