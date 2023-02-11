using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit.Annotation
{
    public abstract class PdfMarkupAnnotation: PdfAnnotation, IMarkupAnnotation
    {
        internal PdfMarkupAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        /// <summary>
        /// Default color is yellow. Edge注释的黄色是#fff066
        /// </summary>
        public Color? StrokeColor { get => GetAnnotColor(); set => SetAnnotColor(value); }

        public PdfRectangleF? Location
        {
            get => GetQuadPoints();
            set
            {
                SetQuadPoint(value.Value);
            }
        }
    }
}
