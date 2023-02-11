using ApplePDF.Extensions;
using ApplePDF.PdfKit.Annotation;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public abstract class PdfAnnotation : IPdfAnnotation
    {
        internal PdfAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) 
        {
            Page = page;
            Annotation = annotation;
            AnnotationType = type;
        }

        public PdfPage Page { get; private set; }

        public PlatformPdfAnnotation Annotation { get; private set; }

        public PdfAnnotationSubtype AnnotationType { get; private set; }

        public PdfRectangleF AnnotBox { get => Annotation.Bounds.ToPdfRectangleF(); set => Annotation.Bounds = value.ToCGRect(); }

        #region AttachmentPoint

        public void SetQuadPoint(PdfRectangleF rect)
        {
            var result = new CoreGraphics.CGPoint[4];
            result[0] = new CoreGraphics.CGPoint(rect.Left, rect.Top);
            result[1] = new CoreGraphics.CGPoint(rect.Right, rect.Top);
            result[2] = new CoreGraphics.CGPoint(rect.Left, rect.Bottom);
            result[3] = new CoreGraphics.CGPoint(rect.Right, rect.Bottom);
            Annotation.QuadrilateralPoints = result;
        }

        public PdfRectangleF? GetQuadPoints()
        {
            var points = Annotation.QuadrilateralPoints;
            if (points.Length == 0)
                return null;
            return PdfRectangleF.FromLTRB((float)points[0].X, (float)points[0].Y, (float)points[3].X, (float)points[3].Y);
        }

        #endregion
    }
}