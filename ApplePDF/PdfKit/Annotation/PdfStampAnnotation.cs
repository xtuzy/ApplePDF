namespace ApplePDF.PdfKit.Annotation
{
    using PDFiumCore;
    using System.Collections.Generic;

    /// <summary>
    /// 图章注释,可以包含文本和图片.
    /// </summary>
    public class PdfStampAnnotation : PdfAnnotation_CanWritePdfPageObj
    {
        private const string TAG = nameof(PdfStampAnnotation);

        /// <summary>
        /// 创建新的Stamp注释
        /// </summary>
        public PdfStampAnnotation()
            : base(PdfAnnotationSubtype.Stamp)
        {
        }

        /// <summary>
        /// After change content of <see cref="PdfPageObj"/>, you need load this method let it's annot update.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpdateObj(PdfPageObj obj)
        {
            return UpdateObjOfAnnot(obj);
        }

        internal PdfStampAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
            
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);

            if (PdfPageObjs != null)
            {
                foreach (var obj in PdfPageObjs)
                {
                    AppendObjToAnnot(obj);
                }
            }
        }
    }
}
