using PDFiumCore;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfSquareAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        public PdfSquareAnnotation()
            : base(PdfAnnotationSubtype.Square)
        {
        }

        internal PdfSquareAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var pdfPageObjs = new PdfPageObj[objectCount];
                PdfPageObjs = pdfPageObjs;
                for (int objIndex = 0; objIndex < objectCount; objIndex++)
                {
                    var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                    if (obj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                        if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            pdfPageObjs[objIndex] = new PdfPagePathObj(obj);
                        }
                    }
                }
            }
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }

    }
}
