using ApplePDF.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLineAnnotation : PdfAnnotation
    {
        const string TAG = nameof(PdfLineAnnotation);

        internal PdfLineAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }

        public Color? StrokeColor { get => Annotation.Color.ToColor(); set => Annotation.Color = value.Value.ToUIColor(); }

        public Color? FillColor { get => Annotation.InteriorColor.ToColor(); set => Annotation.InteriorColor = value.Value.ToUIColor(); }
        
        public (PointF, PointF)? Location
        {
            get
            {
                return (Annotation.StartPoint.ToPointF(), Annotation.EndPoint.ToPointF());
            }
            set
            {
                Annotation.StartPoint = value.Value.Item1.ToCGPoint();
                Annotation.EndPoint = value.Value.Item2.ToCGPoint();
            }
        }
    }
}
