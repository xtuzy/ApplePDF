using PDFiumCore;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace ApplePDF.PdfKit
{
    public class PdfFont : IDisposable
    {
        /// <summary>
        /// 标准字体
        /// </summary>
        static string[] kStandardFontNames = new string[22]
        {
          "Arial",
          "Arial-Bold",
          "Arial-BoldItalic",
          "Arial-Italic",
          "Courier",
          "Courier-BoldOblique",
          "Courier-Oblique",
          "Courier-Bold",
          "CourierNew",
          "CourierNew-Bold",
          "CourierNew-BoldItalic",
          "CourierNew-Italic",
          "Helvetica",
          "Helvetica-Bold",
          "Helvetica-BoldOblique",
          "Helvetica-Oblique",
          "Symbol",
          "TimesNewRoman",
          "TimesNewRoman-Bold",
          "TimesNewRoman-BoldItalic",
          "TimesNewRoman-Italic",
          "ZapfDingbats",
        };

        public FpdfFontT Font { get; private set; }

        public unsafe PdfFont(PdfDocument doc, byte[] fontData, PdfFontType type)
        {
            //byte[] to byte*参考:https://stackoverflow.com/a/6369446/13254773
            fixed (byte* converted = fontData)
            {
                //不明白这里字体类型和CID的不同组合之间的却别
                Font = fpdf_edit.FPDFTextLoadFont(doc.Document, converted, (uint)fontData.Length, (int)type, 0);
            }
        }

        public PdfFont(PdfDocument doc, string fontName)
        {
            if (!kStandardFontNames.Contains(fontName))
                throw new ArgumentException($"fontName:{fontName} not a standard font");
            else
                Font = fpdf_edit.FPDFTextLoadStandardFont(doc.Document, fontName);
        }

        public void Dispose()
        {
            fpdf_edit.FPDFFontClose(Font);
            Font = null;
        }
    }
}
