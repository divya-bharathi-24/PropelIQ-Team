using HealthPlatform.Shared.Extensions;
using HealthPlatform.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedInfrastructure("ClinicalIntelligenceService");

var connectionString = builder.Configuration.GetConnectionString("ClinicalDb")
    ?? throw new InvalidOperationException("ClinicalDb connection string not configured");

builder.Services.AddHealthCheckDefaults(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Clinical Intelligence API", Version = "v1" }));
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
app.Run();
