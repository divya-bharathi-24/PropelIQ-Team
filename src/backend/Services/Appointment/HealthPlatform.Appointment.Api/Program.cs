using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Jobs;
using HealthPlatform.Appointment.Api.Services;
using HealthPlatform.Shared.Extensions;
using HealthPlatform.Shared.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure("AppointmentService");

var connectionString = builder.Configuration.GetConnectionString("AppointmentDb")
    ?? throw new InvalidOperationException("AppointmentDb connection string not configured");

builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHealthCheckDefaults(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Appointment API", Version = "v1" });
});
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Services
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AppointmentManagementService>();
builder.Services.AddScoped<InsuranceCheckService>();
builder.Services.AddScoped<SwapService>();
builder.Services.AddScoped<SwapQueueService>();
builder.Services.AddScoped<CalendarSyncService>();
builder.Services.AddScoped<GoogleCalendarService>();
builder.Services.AddScoped<IcsCalendarService>();
builder.Services.AddSingleton<PdfConfirmationService>();

// Jobs
builder.Services.AddScoped<SlotMonitorJob>();
builder.Services.AddScoped<SwapExpiryJob>();
builder.Services.AddScoped<PdfGenerationJob>();
builder.Services.AddScoped<InsuranceReVerifyJob>();
builder.Services.AddScoped<CalendarSyncJob>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

app.Run();
