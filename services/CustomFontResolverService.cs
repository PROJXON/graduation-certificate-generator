namespace GraduationCertificateGenerator.Services;

using PdfSharpCore.Fonts;
using System.IO;
using System;

public class CustomFontResolver : IFontResolver
{
    public CustomFontResolver() { }
    public string DefaultFontName => "Roboto-Regular";

    public byte[] GetFont(string faceName)
    {
        if (faceName == "Roboto")
        {
            // Load the font file as a byte array
            return File.ReadAllBytes("./assets/fonts/Roboto-Regular.ttf");
        }
        if (faceName == "Geologica")
        {
            return File.ReadAllBytes("./assets/fonts/Geologica-Bold.ttf");
        }
        throw new ArgumentException($"Font {faceName} is not available.");
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName == "Roboto" && !isBold && !isItalic)
        {
            return new FontResolverInfo("Roboto");
        }
        if (familyName == "Geologica" && isBold && !isItalic)
        {
            return new FontResolverInfo("Geologica");
        }
        throw new ArgumentException($"Font {familyName} is not available.");
    }
}
