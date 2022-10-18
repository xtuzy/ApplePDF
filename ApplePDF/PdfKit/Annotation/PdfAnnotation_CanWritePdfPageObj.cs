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
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var pdfPageObjs = new List<PdfPageObj>();
                PdfPageObjs = pdfPageObjs;
                //此处分析注释数据时只当注释只有一个文本和图像对象
                for (int objIndex = 0; objIndex < objectCount; objIndex++)
                {
                    var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, objIndex);
                    if (obj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                        if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                        {
                            pdfPageObjs.Add(new PdfPageTextObj(obj){ Index = objIndex });
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                        {
                            pdfPageObjs.Add(new PdfPageImageObj(obj){ Index = objIndex });
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            pdfPageObjs.Add(new PdfPagePathObj(obj){ Index = objIndex });
                        }
                    }
                }
            }

            if (objectCount == 0)
            {
                // 测试mytest_4_freetextannotation.pdf时,为0时貌似也可能正确,这个注释好像是不显示的
            }
        }

        public IEnumerable<PdfPageObj> PdfPageObjs {  get; set; }
        public int GetObjCount()
        {
            return fpdf_edit.FPDFPageCountObjects(Page.Page);
        }

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
