using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Models;

public sealed record RescheduleRequest(
    [Required] Guid AppointmentId,
    [Required] Guid NewTimeSlotId);
