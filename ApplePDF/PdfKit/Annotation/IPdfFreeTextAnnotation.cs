using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public interface IPdfFreeTextAnnotation
    {
        string Text { get; set; }
        Color? TextColor { get; set; }
    }
}