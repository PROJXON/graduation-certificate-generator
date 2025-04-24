namespace GraduationCertificateGenerator.Services;

using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

public class CertificateService
{
    public static PdfDocument CreatePdf(MainWindow.CertificateData data)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        page.Width = XUnit.FromPoint(2000);
        page.Height = XUnit.FromPoint(1545);
        using var gfx = XGraphics.FromPdfPage(page);

        using var background = XImage.FromFile("./assets/Template.png");
        gfx.DrawImage(background, 0, 0, page.Width, page.Height);


        var smallFont = new XFont("Roboto", 26, XFontStyle.Regular);
        var dateFont = new XFont("Roboto", 24, XFontStyle.Regular);
        var whiteBrush = XBrushes.White;
        var yellowBrush = new XSolidBrush(XColor.FromArgb(255, 215, 0));

        // Draw the course name (max 1400 points wide)
        var courseFont = FitTextToWidth(gfx, data.Course.ToUpper(), "Geologica", XFontStyle.Bold, 900, 90);
        gfx.DrawString(data.Course.ToUpper(), courseFont, yellowBrush, new XPoint(1220, 485), XStringFormats.Center);

        // Draw the participant's name (max 1600 points wide)
        var nameFont = FitTextToWidth(gfx, data.Participant, "Geologica", XFontStyle.Bold, 900, 90);
        gfx.DrawString(data.Participant, nameFont, whiteBrush, new XPoint(1220, 980), XStringFormats.Center);

        gfx.DrawString(data.Participant, nameFont, whiteBrush, new XPoint(1220, 980), XStringFormats.Center);
        gfx.DrawString(data.Course.ToUpper(), courseFont, yellowBrush, new XPoint(1220, 485), XStringFormats.Center);
        gfx.DrawString(data.Date.ToShortDateString(), dateFont, whiteBrush, new XPoint(1480, 1410), XStringFormats.Center);
        gfx.DrawString($"This Certificate is presented to {data.Participant} for their outstanding", smallFont, whiteBrush, new XPoint(1220, 1130), XStringFormats.Center);
        gfx.DrawString($"completion of the {data.Course} course as {data.Role}.", smallFont, whiteBrush, new XPoint(1220, 1170), XStringFormats.Center);

        return document;
    }

    private static XFont FitTextToWidth(XGraphics gfx, string text, string fontName, XFontStyle style, double maxWidth, double initialSize = 65, double minSize = 20)
    {
        double fontSize = initialSize;
        XFont font = new(fontName, fontSize, style);

        while (fontSize > minSize)
        {
            var size = gfx.MeasureString(text, font);
            if (size.Width <= maxWidth)
                break;

            fontSize -= 1;
            font = new XFont(fontName, fontSize, style);
        }

        return font;
    }
}
