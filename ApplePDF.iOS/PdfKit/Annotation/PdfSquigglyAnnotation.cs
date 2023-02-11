using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquigglyAnnotation : PdfMarkupAnnotation
    {
        internal PdfSquigglyAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }
    }
}
