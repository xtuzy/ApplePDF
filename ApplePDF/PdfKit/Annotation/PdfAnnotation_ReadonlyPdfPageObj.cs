using PDFiumCore;
using System.Collections.Generic;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// <see cref="PdfPageObjs"/> 使用<see cref="PdfPageObj"/>[].
    /// </summary>
    public abstract class PdfAnnotation_ReadonlyPdfPageObj : PdfAnnotation, IPdfPageObjAnnotation
    {
        internal PdfAnnotation_ReadonlyPdfPageObj(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        public PdfPageObj[] PdfPageObjs
        {
            get
            {
                return GetAllObj();
            }
        }

        public bool RemoveObj(PdfPageObj obj)
        {
            return RemoveObjOfAnnot(obj);
        }
    }
}
