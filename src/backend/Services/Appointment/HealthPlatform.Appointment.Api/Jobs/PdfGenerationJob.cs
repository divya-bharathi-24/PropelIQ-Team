using HealthPlatform.Appointment.Api.Services;

namespace HealthPlatform.Appointment.Api.Jobs;

public sealed class PdfGenerationJob
{
    private readonly PdfConfirmationService _pdfService;

    public PdfGenerationJob(PdfConfirmationService pdfService)
    {
        _pdfService = pdfService;
    }

    /// <summary>
    /// Retries PDF generation for appointments where initial generation failed.
    /// </summary>
    public byte[] Execute(Guid appointmentId, DateTime scheduledAt, string? reason, Guid providerId)
    {
        return _pdfService.GenerateConfirmationPdf(appointmentId, scheduledAt, reason, providerId);
    }
}
