using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfUnderlineAnnotation : PdfAnnotation
    {
        public PdfUnderlineAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfUnderlineAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
