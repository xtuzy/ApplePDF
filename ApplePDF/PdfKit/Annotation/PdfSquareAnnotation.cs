using PDFiumCore;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        internal PdfSquareAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }
    }
}
