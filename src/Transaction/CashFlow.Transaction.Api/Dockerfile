FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json CashFlow.slnx ./
COPY src/CashFlow.Shared.Contracts/CashFlow.Shared.Contracts.csproj src/CashFlow.Shared.Contracts/
COPY src/Transaction/CashFlow.Transaction.Domain/CashFlow.Transaction.Domain.csproj src/Transaction/CashFlow.Transaction.Domain/
COPY src/Transaction/CashFlow.Transaction.Application/CashFlow.Transaction.Application.csproj src/Transaction/CashFlow.Transaction.Application/
COPY src/Transaction/CashFlow.Transaction.Infrastructure/CashFlow.Transaction.Infrastructure.csproj src/Transaction/CashFlow.Transaction.Infrastructure/
COPY src/Transaction/CashFlow.Transaction.Api/CashFlow.Transaction.Api.csproj src/Transaction/CashFlow.Transaction.Api/

RUN dotnet restore src/Transaction/CashFlow.Transaction.Api/CashFlow.Transaction.Api.csproj

COPY . .

RUN dotnet publish src/Transaction/CashFlow.Transaction.Api/CashFlow.Transaction.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CashFlow.Transaction.Api.dll"]
