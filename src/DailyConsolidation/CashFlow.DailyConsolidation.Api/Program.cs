using CashFlow.DailyConsolidation.Application;
using CashFlow.DailyConsolidation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
                .AddApplication()
                .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CashFlow Daily Consolidation API v1");
    });
}

app.UseHttpsRedirection();

app.Run();
