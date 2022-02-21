using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfStampAnnotation : PdfAnnotation
    {
        public PdfStampAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfStampAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
