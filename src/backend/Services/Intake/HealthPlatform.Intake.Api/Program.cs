using HealthPlatform.Shared.Extensions;
using HealthPlatform.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedInfrastructure("IntakeService");

var connectionString = builder.Configuration.GetConnectionString("IntakeDb")
    ?? throw new InvalidOperationException("IntakeDb connection string not configured");

builder.Services.AddHealthCheckDefaults(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Intake API", Version = "v1" }));
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
app.Run();
