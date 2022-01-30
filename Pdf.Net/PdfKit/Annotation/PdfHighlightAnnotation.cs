using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfHighlightAnnotation : PdfAnnotation
    {
        public PdfHighlightAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfHighlightAnnotation(PdfPage page,FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page,annotation,type, index)
        {
        }

        public void AppendAnnotationPoint(RectangleF rect)
        {
            var success = fpdf_annot.FPDFAnnotAppendAttachmentPoints(Annotation, new FS_QUADPOINTSF() 
            {
                X1=rect.Left,
                Y1=rect.Top,
                X2=rect.Right,
                Y2=rect.Top,
                X3=rect.Left,
                Y3=rect.Bottom,
                X4=rect.Right,
                Y4=rect.Bottom
            })==1;
            if (success) ;
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
