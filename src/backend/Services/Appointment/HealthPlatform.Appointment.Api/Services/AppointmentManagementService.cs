using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using HealthPlatform.Appointment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class AppointmentManagementService
{
    private readonly AppointmentDbContext _db;
    private const int PageSize = 10;
    private const int MaxDailyCancellations = 3;

    public AppointmentManagementService(AppointmentDbContext db)
    {
        _db = db;
    }

    public async Task<AppointmentListResponse> GetAppointmentsAsync(
        Guid patientId, int page, string? statusFilter, string sortBy, bool descending, CancellationToken ct)
    {
        var query = _db.Appointments.Where(a => a.PatientId == patientId);

        if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<AppointmentStatus>(statusFilter, true, out var status))
        {
            query = query.Where(a => a.Status == status);
        }

        query = sortBy?.ToLowerInvariant() switch
        {
            "status" => descending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            _ => descending ? query.OrderByDescending(a => a.ScheduledAt) : query.OrderBy(a => a.ScheduledAt),
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(a => new AppointmentListItem(
                a.Id, a.ProviderId, a.ScheduledAt,
                a.Status.ToString(), a.ReasonForVisit, a.RescheduleCount))
            .ToListAsync(ct);

        return new AppointmentListResponse(items, totalCount, page, PageSize);
    }

    public async Task<(bool Success, string Message)> RescheduleAsync(
        Guid patientId, RescheduleRequest request, CancellationToken ct)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.PatientId == patientId, ct);

        if (appointment is null)
            return (false, "Appointment not found.");

        if (appointment.Status != AppointmentStatus.Scheduled)
            return (false, "Only scheduled appointments can be rescheduled.");

        var newSlot = await _db.TimeSlots.FirstOrDefaultAsync(s => s.Id == request.NewTimeSlotId && s.IsAvailable, ct);
        if (newSlot is null)
            return (false, "Selected slot is no longer available.");

        // Release original slot
        var oldSlot = await _db.TimeSlots.FirstOrDefaultAsync(s => s.Id == appointment.TimeSlotId, ct);
        if (oldSlot is not null)
        {
            oldSlot.IsAvailable = true;
        }

        // Reserve new slot
        newSlot.IsAvailable = false;

        appointment.TimeSlotId = newSlot.Id;
        appointment.ScheduledAt = newSlot.StartTime;
        appointment.RescheduleCount++;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        var hoursUntil = (appointment.ScheduledAt - DateTime.UtcNow).TotalHours;
        var warning = hoursUntil < 24 ? " Late reschedule may affect reliability score." : "";

        return (true, $"Appointment rescheduled successfully.{warning}");
    }

    public async Task<(bool Success, string Message)> CancelAsync(
        Guid patientId, Guid appointmentId, string? reason, CancellationToken ct)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId, ct);

        if (appointment is null)
            return (false, "Appointment not found.");

        if (appointment.Status != AppointmentStatus.Scheduled)
            return (false, "Only scheduled appointments can be cancelled.");

        // Check daily cancellation limit
        var todayStart = DateTime.UtcNow.Date;
        var dailyCancellations = await _db.Appointments.CountAsync(
            a => a.PatientId == patientId
                 && a.Status == AppointmentStatus.Cancelled
                 && a.UpdatedAt >= todayStart,
            ct);

        if (dailyCancellations >= MaxDailyCancellations)
            return (false, "Maximum daily cancellations reached. Please contact staff for assistance.");

        var hoursUntil = (appointment.ScheduledAt - DateTime.UtcNow).TotalHours;

        // Release the slot
        var slot = await _db.TimeSlots.FirstOrDefaultAsync(s => s.Id == appointment.TimeSlotId, ct);
        if (slot is not null)
        {
            slot.IsAvailable = true;
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        var warning = hoursUntil < 24 ? " Late cancellation may affect your reliability score." : "";
        return (true, $"Appointment cancelled.{warning}");
    }
}
