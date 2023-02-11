using PDFiumCore;
using System.Collections.Generic;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// <see cref="PdfPageObjs"/> 使用<see cref="List{T}"/>.
    /// </summary>
    public abstract class PdfAnnotation_CanWritePdfPageObj : PdfAnnotation_ReadonlyPdfPageObj
    {
        internal PdfAnnotation_CanWritePdfPageObj(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
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

        public bool AppendObj(PdfPageObj obj)
        {
            return AppendObjToAnnot(obj);
        }
    }
}
