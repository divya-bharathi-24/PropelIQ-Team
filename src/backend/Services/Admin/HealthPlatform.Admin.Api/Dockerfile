FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/Admin/HealthPlatform.Admin.Api/HealthPlatform.Admin.Api.csproj", "Services/Admin/HealthPlatform.Admin.Api/"]
COPY ["Shared/HealthPlatform.Shared/HealthPlatform.Shared.csproj", "Shared/HealthPlatform.Shared/"]
RUN dotnet restore "Services/Admin/HealthPlatform.Admin.Api/HealthPlatform.Admin.Api.csproj"
COPY . .
WORKDIR "/src/Services/Admin/HealthPlatform.Admin.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "HealthPlatform.Admin.Api.dll"]
