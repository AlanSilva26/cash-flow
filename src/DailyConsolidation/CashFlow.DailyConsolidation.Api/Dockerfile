FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json CashFlow.slnx ./
COPY src/CashFlow.Shared.Contracts/CashFlow.Shared.Contracts.csproj src/CashFlow.Shared.Contracts/
COPY src/DailyConsolidation/CashFlow.DailyConsolidation.Application/CashFlow.DailyConsolidation.Application.csproj src/DailyConsolidation/CashFlow.DailyConsolidation.Application/
COPY src/DailyConsolidation/CashFlow.DailyConsolidation.Infrastructure/CashFlow.DailyConsolidation.Infrastructure.csproj src/DailyConsolidation/CashFlow.DailyConsolidation.Infrastructure/
COPY src/DailyConsolidation/CashFlow.DailyConsolidation.Api/CashFlow.DailyConsolidation.Api.csproj src/DailyConsolidation/CashFlow.DailyConsolidation.Api/

RUN dotnet restore src/DailyConsolidation/CashFlow.DailyConsolidation.Api/CashFlow.DailyConsolidation.Api.csproj

COPY . .

RUN dotnet publish src/DailyConsolidation/CashFlow.DailyConsolidation.Api/CashFlow.DailyConsolidation.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CashFlow.DailyConsolidation.Api.dll"]
