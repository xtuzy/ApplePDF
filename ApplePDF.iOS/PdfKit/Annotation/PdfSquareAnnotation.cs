using ApplePDF.Extensions;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation
    {
        internal PdfSquareAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }

        public Color? StrokeColor { get => Annotation.Color.ToColor(); set => Annotation.Color = value.Value.ToUIColor(); }

        public Color? FillColor { get => Annotation.InteriorColor.ToColor(); set => Annotation.InteriorColor = value.Value.ToUIColor(); }
    }
}
