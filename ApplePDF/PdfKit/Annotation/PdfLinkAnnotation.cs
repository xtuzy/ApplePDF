using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLinkAnnotation : PdfAnnotation, IPdfLinkAnnotation
    {
        internal PdfLinkAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
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
