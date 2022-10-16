using PDFiumCore;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        public PdfSquareAnnotation()
            : base(PdfAnnotationSubtype.Square)
        {
        }

        internal PdfSquareAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }
    }
}
