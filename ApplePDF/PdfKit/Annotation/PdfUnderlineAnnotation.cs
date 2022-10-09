using PDFiumCore;
using System;
using System.Collections.Generic;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfUnderlineAnnotation : PdfAnnotation
    {
        List<PdfRectangleF> UnderlineLocation = new List<PdfRectangleF>();
        public PdfUnderlineAnnotation()
            : base(PdfAnnotationSubtype.Underline)
        {
        }

        internal PdfUnderlineAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            var count = (int)fpdf_annot.FPDFAnnotCountAttachmentPoints(Annotation);
            var success = count == 0;
            if (!success) throw new NotImplementedException("No highlight points");
            var point = new FS_QUADPOINTSF();
            for (var i = 0; i < count; i++)
            {
                success = fpdf_annot.FPDFAnnotGetAttachmentPoints(Annotation, (ulong)i, point) == 1;
                if (!success) throw new NotImplementedException("Get highlight point fail");
                var rect = PdfRectangleF.FromLTRB(point.X1, point.Y1, point.X4, point.Y4);
                UnderlineLocation.Add(rect);
            }
        }

        public void AppendAnnotationPoint(PdfRectangleF rect)
        {
            var success = fpdf_annot.FPDFAnnotAppendAttachmentPoints(Annotation, new FS_QUADPOINTSF()
            {
                X1 = rect.Left,
                Y1 = rect.Top,
                X2 = rect.Right,
                Y2 = rect.Top,
                X3 = rect.Left,
                Y3 = rect.Bottom,
                X4 = rect.Right,
                Y4 = rect.Bottom
            }) == 1;
            if (success) UnderlineLocation.Add(rect);
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            AppendAnnotationPoint(this.AnnotBox);
        }
    }
}
