using HealthPlatform.Notification.Api.Services;
using HealthPlatform.Shared.Extensions;
using HealthPlatform.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedInfrastructure("NotificationService");

var connectionString = builder.Configuration.GetConnectionString("NotificationDb")
    ?? throw new InvalidOperationException("NotificationDb connection string not configured");

builder.Services.AddHealthCheckDefaults(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Notification API", Version = "v1" }));
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<AppointmentEmailService>();
builder.Services.AddScoped<SwapNotificationService>();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
app.Run();
