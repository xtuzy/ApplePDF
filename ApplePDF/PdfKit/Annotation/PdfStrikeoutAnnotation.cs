using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfStrikeoutAnnotation : PdfMarkupAnnotation
    {
        internal PdfStrikeoutAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }
    }
}
