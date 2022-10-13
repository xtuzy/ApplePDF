using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLinkAnnotation : PdfAnnotation, IColorAnnotation
    {
        public PdfLinkAnnotation() : base(PdfAnnotationSubtype.Link)
        {
        }

        internal PdfLinkAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
            AnnotColor = GetAnnotColor();
        }

        public Color? AnnotColor { get; set; }

        public int Link { get; set; }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
