namespace ApplePDF.PdfKit.Annotation
{
    using System.Collections.Generic;

    /// <summary>
    /// 图章注释,可以包含文本和图片.
    /// </summary>
    public class PdfStampAnnotation : PdfAnnotation
    {
        private const string TAG = nameof(PdfStampAnnotation);
        internal PdfStampAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type)
            : base(page, annotation, type)
        {
        }
    }
}
