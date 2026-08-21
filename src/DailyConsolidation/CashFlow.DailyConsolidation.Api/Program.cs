using CashFlow.DailyConsolidation.Api.Endpoints;
using CashFlow.DailyConsolidation.Api.Errors;
using CashFlow.DailyConsolidation.Api.Extensions;
using CashFlow.DailyConsolidation.Application;
using CashFlow.DailyConsolidation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration)
                .AddApplication()
                .AddOpenApi();

var app = builder.Build();

await app.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CashFlow Daily Consolidation API v1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapDailyBalanceEndpoints();

app.Run();
