using PDFiumCore;
using System.Collections.Generic;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// <see cref="PdfPageObjs"/> 使用<see cref="List{T}"/>.
    /// </summary>
    public abstract class PdfAnnotation_CanWritePdfPageObj : PdfAnnotation, IPdfPageObjAnnotation
    {
        protected PdfAnnotation_CanWritePdfPageObj(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfAnnotation_CanWritePdfPageObj(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        public IEnumerable<PdfPageObj> PdfPageObjs {  get; set; }

        bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (PdfPageObjs != null)
                {
                    var pdfPageObjs = PdfPageObjs as List<PdfPageObj>;
                    if (pdfPageObjs != null)
                    {
                        for (int index = 0; index < pdfPageObjs.Count; index++)
                        {
                            pdfPageObjs[index].Dispose();
                            pdfPageObjs[index] = null;
                        }
                    }
                }
            }
            PdfPageObjs = null;
            base.Dispose(disposing);
            _disposed = true;
        }
    }
}
