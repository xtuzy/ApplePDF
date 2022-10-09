using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLinkAnnotation : PdfAnnotation
    {
        public PdfLinkAnnotation() : base(PdfAnnotationSubtype.Link)
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
