using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfHighlightAnnotation : PdfAnnotation, IDefaultColorAnnotation
    {
        List<PdfRectangleF> HighlightLocation = new List<PdfRectangleF>();

        public PdfPopupAnnotation PopupAnnotation;

        public PdfHighlightAnnotation()
            : base(PdfAnnotationSubtype.Highlight)
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
                var rect = PdfRectangleF.FromLTRB(point.X1, point.Y1, point.X4, point.Y4);
                HighlightLocation.Add(rect);
            }

            // 获取附着在其上的Popup注释
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

            // 先尝试最简单的方式获取颜色
            AnnotColor = GetAnnotColor();
            if (AnnotColor == null)
            {
                // 尝试使用对象来获取颜色,Edge标注的存放在这里
                AnnotColor = GetFillAndStrokeColor().FillColor;
            }
        }

        /// <summary>
        /// Default color is yellow. Edge注释的黄色是#fff066
        /// </summary>
        public Color? AnnotColor
        {
            get;
            set;
        }

        public void AppendAnnotationPoint(PdfRectangleF rect)
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

            // 设置颜色,我们不管其它软件是否使用对象来设置颜色,我们用最简单的方式
            if (AnnotColor != null)
            {
                var success = fpdf_annot.FPDFAnnotSetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, AnnotColor.Value.R, AnnotColor.Value.G, AnnotColor.Value.B, AnnotColor.Value.A) == 1;
                if (!success)
                    throw new NotImplementedException($"{this.GetType()}:Set AnnotColor fail, Fails when called on annotations with appearance streams already defined; instead use FPDFPath_Set(Stroke|Fill)Color().");
            }

            AppendAnnotationPoint(this.AnnotBox);
        }
    }
}
