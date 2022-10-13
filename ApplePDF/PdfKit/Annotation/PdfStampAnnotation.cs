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
                            pdfPageObjs.Add(new PdfPageTextObj(obj));
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                        {
                            pdfPageObjs.Add(new PdfPageImageObj(obj));
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            pdfPageObjs.Add(new PdfPagePathObj(obj));
                        }
                    }
                }
            }

            if (objectCount == 0)
            {
                // 测试mytest_4_freetextannotation.pdf时,为0时貌似也可能正确,这个注释好像是不显示的
            }
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
