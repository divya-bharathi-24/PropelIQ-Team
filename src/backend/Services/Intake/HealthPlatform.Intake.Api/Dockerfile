FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Intake/HealthPlatform.Intake.Api/HealthPlatform.Intake.Api.csproj", "Services/Intake/HealthPlatform.Intake.Api/"]
COPY ["Shared/HealthPlatform.Shared/HealthPlatform.Shared.csproj", "Shared/HealthPlatform.Shared/"]
RUN dotnet restore "Services/Intake/HealthPlatform.Intake.Api/HealthPlatform.Intake.Api.csproj"
COPY . .
WORKDIR "/src/Services/Intake/HealthPlatform.Intake.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "HealthPlatform.Intake.Api.dll"]
