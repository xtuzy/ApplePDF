using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfCircleAnnotation : PdfAnnotation
    {
        internal PdfCircleAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
            
        }

        public Color? StrokeColor { get => GetAnnotColor(); set => SetAnnotColor(value); }
    }
}
