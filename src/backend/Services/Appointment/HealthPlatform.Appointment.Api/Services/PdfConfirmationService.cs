using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class PdfConfirmationService
{
    static PdfConfirmationService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateConfirmationPdf(
        Guid appointmentId, DateTime scheduledAt, string? reasonForVisit, Guid providerId)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(
            $"checkin:{appointmentId}", QRCodeGenerator.ECCLevel.M);
        var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(5);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);

                page.Header().Text("Appointment Confirmation")
                    .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text($"Appointment ID: {appointmentId}").FontSize(10);
                    col.Item().Text($"Date & Time: {scheduledAt:MMMM dd, yyyy h:mm tt} UTC").FontSize(12);
                    col.Item().Text($"Provider ID: {providerId}").FontSize(10);
                    col.Item().Text($"Reason: {reasonForVisit ?? "General consultation"}").FontSize(10);
                    col.Item().PaddingTop(15).AlignCenter().Width(120).Image(qrBytes);
                    col.Item().AlignCenter().Text("Scan for check-in").FontSize(8).Italic();
                });

                page.Footer().AlignCenter()
                    .Text($"Generated {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(8);
            });
        }).GeneratePdf();
    }
}
