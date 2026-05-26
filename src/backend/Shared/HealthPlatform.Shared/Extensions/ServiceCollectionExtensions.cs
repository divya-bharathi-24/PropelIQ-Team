using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

namespace HealthPlatform.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddSharedInfrastructure(
        this WebApplicationBuilder builder, string serviceName)
    {
        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Seq(
                    context.Configuration["Seq:Url"] ?? "http://localhost:5341",
                    restrictedToMinimumLevel: LogEventLevel.Information);
        });

        return builder;
    }

    public static IServiceCollection AddHealthCheckDefaults(
        this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "ready" })
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

        return services;
    }
}
