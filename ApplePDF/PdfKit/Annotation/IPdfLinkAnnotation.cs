using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public interface IPdfLinkAnnotation
    {
        Color? AnnotColor { get; set; }
        int Link { get; set; }
        PdfRectangleF? Location { get; set; }
    }
}