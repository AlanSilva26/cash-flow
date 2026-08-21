using CashFlow.Transaction.Api.Endpoints;
using CashFlow.Transaction.Api.Errors;
using CashFlow.Transaction.Api.Extensions;
using CashFlow.Transaction.Application;
using CashFlow.Transaction.Infrastructure;

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
        options.SwaggerEndpoint("/openapi/v1.json", "CashFlow Transaction API v1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapTransactionEndpoints();

app.Run();
