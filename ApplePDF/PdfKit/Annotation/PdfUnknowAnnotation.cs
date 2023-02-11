using PDFiumCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// 我们假定未知的注释拥有全部功能
    /// </summary>
    public class PdfUnknowAnnotation : PdfAnnotation_CanWritePdfPageObj
    {
        internal PdfUnknowAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        public string CustomAnnotationType
        {
            get => GetStringValueFromKey(Constant.CommonKey.kSubtype);
            set => SetStringValueForKey(value, Constant.CommonKey.kSubtype);
        }

        public Color? AnnotColor { get => GetAnnotColor(); set => SetAnnotColor(value); }
    }
}
