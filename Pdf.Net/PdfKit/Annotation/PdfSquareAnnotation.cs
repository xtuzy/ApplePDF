using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation
    {
        public PdfSquareAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfSquareAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
