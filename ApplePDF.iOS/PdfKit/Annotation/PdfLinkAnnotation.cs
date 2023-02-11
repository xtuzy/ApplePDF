using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLinkAnnotation : PdfAnnotation, IPdfLinkAnnotation
    {
        internal PdfLinkAnnotation(PdfPage page,PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page,annotation,type)
        {
        }

        public Color? AnnotColor { get; set; }

        public int Link { get; set; }

        public PdfRectangleF? Location
        {
            get => GetQuadPoints();
            set => SetQuadPoint(value.Value);
        }
    }
}
