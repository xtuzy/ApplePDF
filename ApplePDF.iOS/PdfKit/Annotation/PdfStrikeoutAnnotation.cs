using ApplePDF.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfStrikeoutAnnotation : PdfMarkupAnnotation
    {
        internal PdfStrikeoutAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }
    }
}
