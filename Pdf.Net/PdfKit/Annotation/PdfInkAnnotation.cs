using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfInkAnnotation : PdfAnnotation
    {
        public PdfInkAnnotation(PdfPage page, PdfAnnotationSubtype type) : base(page, type)
        {
        }

        internal PdfInkAnnotation(PdfPage page, int index) : base(page, index)
        {
        }
    }
}
