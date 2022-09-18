using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfWidgetAnnotation : PdfAnnotation
    {
        public PdfWidgetAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfWidgetAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
