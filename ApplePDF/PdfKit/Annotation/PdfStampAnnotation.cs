namespace ApplePDF.PdfKit.Annotation
{
    using PDFiumCore;
    using System.Collections.Generic;

    /// <summary>
    /// 图章注释,可以包含文本和图片.
    /// </summary>
    public class PdfStampAnnotation : PdfAnnotation_CanWritePdfPageObj
    {
        internal PdfStampAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
        }
    }
}
