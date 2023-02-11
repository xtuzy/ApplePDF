using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfWidgetAnnotation : PdfAnnotation
    {
        internal PdfWidgetAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index) { }
    }
}
