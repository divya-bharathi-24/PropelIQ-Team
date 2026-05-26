FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG SERVICE_NAME=Auth
ARG SERVICE_PROJECT=HealthPlatform.Auth.Api
WORKDIR /src

COPY ["Services/${SERVICE_NAME}/${SERVICE_PROJECT}/${SERVICE_PROJECT}.csproj", "Services/${SERVICE_NAME}/${SERVICE_PROJECT}/"]
COPY ["Shared/HealthPlatform.Shared/HealthPlatform.Shared.csproj", "Shared/HealthPlatform.Shared/"]
RUN dotnet restore "Services/${SERVICE_NAME}/${SERVICE_PROJECT}/${SERVICE_PROJECT}.csproj"

COPY . .
WORKDIR "/src/Services/${SERVICE_NAME}/${SERVICE_PROJECT}"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "HealthPlatform.Auth.Api.dll"]
