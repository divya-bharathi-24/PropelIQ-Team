using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class IcsCalendarService
{
    private readonly AppointmentDbContext _db;

    public IcsCalendarService(AppointmentDbContext db)
    {
        _db = db;
    }

    public string GenerateIcsForAppointment(Data.Entities.Appointment appointment)
    {
        var calendar = new Calendar();
        calendar.AddChild(CreateCalendarEvent(appointment));

        var serializer = new CalendarSerializer();
        return serializer.SerializeToString(calendar);
    }

    public async Task<string> GenerateIcsFeedAsync(Guid patientId, CancellationToken ct)
    {
        var appointments = await _db.Appointments
            .Where(a => a.PatientId == patientId &&
                        (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Completed) &&
                        (a.ScheduledAt >= DateTime.UtcNow || a.Status == AppointmentStatus.Completed))
            .OrderByDescending(a => a.ScheduledAt)
            .ToListAsync(ct);

        // Future confirmed + last 5 completed
        var future = appointments.Where(a => a.ScheduledAt >= DateTime.UtcNow).ToList();
        var recentCompleted = appointments
            .Where(a => a.Status == AppointmentStatus.Completed)
            .Take(5)
            .ToList();

        var calendar = new Calendar();
        calendar.Properties.Add(new CalendarProperty("X-WR-CALNAME", "Healthcare Appointments"));

        foreach (var appt in future.Union(recentCompleted))
        {
            calendar.AddChild(CreateCalendarEvent(appt));
        }

        var serializer = new CalendarSerializer();
        return serializer.SerializeToString(calendar);
    }

    private static CalendarEvent CreateCalendarEvent(Data.Entities.Appointment appointment)
    {
        return new CalendarEvent
        {
            Uid = appointment.Id.ToString(),
            Summary = $"Healthcare Appointment",
            Description = appointment.ReasonForVisit ?? "Medical appointment",
            DtStart = new CalDateTime(appointment.ScheduledAt, "UTC"),
            DtEnd = new CalDateTime(appointment.ScheduledAt.AddMinutes(30), "UTC"),
            Status = appointment.Status == AppointmentStatus.Cancelled ? "CANCELLED" : "CONFIRMED",
        };
    }
}
