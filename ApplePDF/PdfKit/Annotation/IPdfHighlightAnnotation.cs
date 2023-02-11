using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public interface IPdfHighlightAnnotation : IMarkupAnnotation
    {
        Color? HighlightColor { get; set; }
        PdfRectangleF? Location { get; set; }
        PdfPopupAnnotation PopupAnnotation { get; }

        PdfPopupAnnotation AddPopupAnnotation();
    }
}