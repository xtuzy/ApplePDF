using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfHighlightAnnotation : PdfAnnotation
    {
        List<RectangleF> HighlightLocation = new List<RectangleF>();

        public PdfPopupAnnotation PopupAnnotation;

        public PdfHighlightAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfHighlightAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            var count = (int)fpdf_annot.FPDFAnnotCountAttachmentPoints(Annotation);
            var success = count != 0;
            if (!success) throw new NotImplementedException("No highlight points");
            var point = new FS_QUADPOINTSF();
            for (var i = 0; i < count; i++)
            {
                success = fpdf_annot.FPDFAnnotGetAttachmentPoints(Annotation, (ulong)i, point) == 1;
                if (!success) throw new NotImplementedException("Get highlight point fail");
                var rect = new RectangleF(point.X1, point.Y1, point.X4 - point.X1, point.Y4 - point.Y1);
                HighlightLocation.Add(rect);
            }

            //获取附着在其上的Popup注释
            var havePopup = fpdf_annot.FPDFAnnotHasKey(annotation, PdfPopupAnnotation.kPopupKey);
            if (havePopup == 1)
            {
                PopupAnnotation = new PdfPopupAnnotation(this);
                // Get Text
                var buffer = new ushort[100];
                var result = fpdf_annot.FPDFAnnotGetStringValue(annotation, ConstDictionaryKeyContents, ref buffer[0], (uint)buffer.Length);
                if (result == 0)
                {
                    throw new NotImplementedException();
                }
                else if (result == 2)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    unsafe
                    {
                        fixed (ushort* dataPtr = &buffer[0])
                        {
                            PopupAnnotation.Text = new string((char*)dataPtr, 0, (int)result);
                        }
                    }
                }
            }
        }

        public void AppendAnnotationPoint(RectangleF rect)
        {
            var success = fpdf_annot.FPDFAnnotAppendAttachmentPoints(Annotation, new FS_QUADPOINTSF()
            {
                X1 = rect.Left,
                Y1 = rect.Top,
                X2 = rect.Right,
                Y2 = rect.Top,
                X3 = rect.Left,
                Y3 = rect.Bottom,
                X4 = rect.Right,
                Y4 = rect.Bottom
            }) == 1;
            if (success) HighlightLocation.Add(rect);
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            AppendAnnotationPoint(this.AnnotBox);
        }
    }
}
