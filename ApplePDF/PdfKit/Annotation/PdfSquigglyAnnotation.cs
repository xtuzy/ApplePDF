using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquigglyAnnotation : PdfMarkupAnnotation
    {
        internal PdfSquigglyAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }
    }
}
