using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public interface IMarkupAnnotation
    {
         PdfRectangleF? Location { get; }
    }
}
