using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfFreeTextAnnotation : PdfAnnotation
    {
        public PdfFreeTextAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfFreeTextAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        public string Text { get; set; }


        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }
    }
}
