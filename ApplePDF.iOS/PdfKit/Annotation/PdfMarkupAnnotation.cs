using ApplePDF.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// See <see cref="PdfHighlightAnnotation"/>, <see cref="PdfSquigglyAnnotation"/>, <see cref="PdfStrikeoutAnnotation"/>, <see cref="PdfUnderlineAnnotation"/>
    /// </summary>
    public abstract class PdfMarkupAnnotation: PdfAnnotation, IMarkupAnnotation
    {
        internal PdfMarkupAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }

        public PdfRectangleF? Location
        {
            get => GetQuadPoints();
            set => SetQuadPoint(value.Value);
        }

        public Color? StrokeColor { get => Annotation.Color.ToColor(); set => Annotation.Color = value.Value.ToUIColor(); }

        public Color? FillColor { get => Annotation.InteriorColor.ToColor(); set => Annotation.InteriorColor = value.Value.ToUIColor(); }
    }
}
