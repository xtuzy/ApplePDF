using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfUnderlineAnnotation : PdfMarkupAnnotation
    {
        internal PdfUnderlineAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }
    }
}
