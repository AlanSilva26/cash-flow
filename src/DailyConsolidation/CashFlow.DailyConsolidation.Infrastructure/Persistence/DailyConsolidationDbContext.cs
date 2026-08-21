using CashFlow.DailyConsolidation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.DailyConsolidation.Infrastructure.Persistence;

public sealed class DailyConsolidationDbContext(
    DbContextOptions<DailyConsolidationDbContext> options
) : DbContext(options)
{
    public DbSet<DailyBalance> DailyBalances => Set<DailyBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DailyConsolidationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}