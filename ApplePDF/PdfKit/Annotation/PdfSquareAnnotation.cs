using PDFiumCore;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation, IFillColorAnnotation, IStrokeColorAnnotation
    {
        public PdfSquareAnnotation()
            : base(PdfAnnotationSubtype.Square)
        {
        }

        internal PdfSquareAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            var colors = GetFillAndStrokeColor();
            FillColor = colors.FillColor;
            StrokeColor = colors.StrokeColor;
        }

        public Color? FillColor { get; private set; }

        public Color? StrokeColor { get; private set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }
    }
}
