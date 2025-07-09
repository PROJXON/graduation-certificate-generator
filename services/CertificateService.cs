namespace GraduationCertificateGenerator.Services;

using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Globalization;

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


        var whiteBrush = XBrushes.White;

        var smallFont = new XFont("Roboto", 30, XFontStyle.Regular);
        var dateFont = new XFont("Roboto", 38, XFontStyle.Regular);
        var nameFont = FitTextToWidth(gfx, data.Participant, "Geologica", XFontStyle.Bold, 900, 120);

        gfx.DrawString(data.Participant, nameFont, whiteBrush, new XPoint(990, 1010), XStringFormats.Center);
        gfx.DrawString($"This Certificate is presented to {data.Participant} for their outstanding completion of", smallFont, whiteBrush, new XPoint(990, 1160), XStringFormats.Center);
        gfx.DrawString(
            $"the Momentum Internship Program as {data.Role} from {data.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)} to {data.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}.",
            smallFont, whiteBrush, new XPoint(990, 1200), XStringFormats.Center);
        gfx.DrawString(data.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), dateFont, whiteBrush, new XPoint(1285, 1325), XStringFormats.Center);

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
