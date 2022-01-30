using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfLinkAnnotation : PdfAnnotation
    {
        public PdfLinkAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfLinkAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
            //fpdf_doc.
        }

        public int Link { get; set; }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
