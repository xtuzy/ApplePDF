using CoreGraphics;
using Foundation;
using System;

namespace ApplePDF.PdfKit
{
    public class PdfFont : IPdfFont
    {
        public PlatformPdfFont Font { get; private set; }

        public string Name { get; private set; }

        public PdfFont(PdfDocument doc, byte[] fontData)
        {
           var data = new CGDataProvider(fontData);
           Font = CGFont.CreateFromProvider(data);
            Name = Font.FullName;
        }

        public PdfFont(PdfDocument doc, string fontName)
        {
            Name = fontName;
            Font = CGFont.CreateWithFontName(fontName);
        }

        public PdfFont(PlatformPdfFont fpdfFont)
        {
            Font = fpdfFont;
            Name = Font.FullName;
        }
    }
}