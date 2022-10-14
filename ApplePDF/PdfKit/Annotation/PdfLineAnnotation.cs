using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLineAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        const string TAG = nameof(PdfLineAnnotation);
        public PdfLineAnnotation()
            : base(PdfAnnotationSubtype.Line)
        {
        }

        internal PdfLineAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            //Location
            FS_POINTF_ start = new FS_POINTF_();
            FS_POINTF_ end = new FS_POINTF_();
            bool success = false;
            success = fpdf_annot.FPDFAnnotGetLine(Annotation, start, end) == 1;
            if (success)
            {
                StartLocation = new PointF(start.X, start.Y);
                EndLocation = new PointF(end.X, end.Y);
            }

            //More line detail,比如箭头
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

        public PointF StartLocation { get; private set; }
        public PointF EndLocation { get; private set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }
    }
}
