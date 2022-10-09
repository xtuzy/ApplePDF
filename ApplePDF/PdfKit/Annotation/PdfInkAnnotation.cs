using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// How to use:
    /// If you want add a new PdfInkAnnotation to Page, you can set <see cref="Inks"/>, then 
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation, IDefaultColorAnnotation
    {
        public PdfInkAnnotation() : base(PdfAnnotationSubtype.Ink)
        {
        }

        internal PdfInkAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
            AnnotColor = GetAnnotColor();
            if (AnnotColor == null)
                AnnotColor = GetFillAndStrokeColor().StrokeColor;
        }

        List<List<PointF>> inks;
        public List<List<PointF>> Inks
        {
            get { if (inks == null) GetInks(); return inks; }
            set { inks = value; }
        }

        /// <summary>
        /// Ink的颜色我在测试的Pdf上是通过StrokeColor获取的,但看Pdfium的测试使用的是这个
        /// </summary>
        public Color? AnnotColor { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            foreach (var i in Inks)
            {
                AddInk(i);
            }
            // 设置颜色,我们不管其它软件是否使用对象来设置颜色,我们用最简单的方式
            if (AnnotColor != null)
            {
                var success = fpdf_annot.FPDFAnnotSetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, AnnotColor.Value.R, AnnotColor.Value.G, AnnotColor.Value.B, AnnotColor.Value.A) == 1;
                if (!success)
                    throw new NotImplementedException($"{this.GetType()}:Set AnnotColor fail, Fails when called on annotations with appearance streams already defined; instead use FPDFPath_Set(Stroke|Fill)Color().");
            }
        }

        public int AddInk(List<PointF> ink)
        {
            FS_POINTF_[] points = new FS_POINTF_[ink.Count];
            for (var index = 0; index < ink.Count; index++)
            {
                points[index] = new FS_POINTF_() { X = ink[index].X, Y = ink[index].Y };
            }
            var success = fpdf_annot.FPDFAnnotAddInkStroke(Annotation, points[0], (ulong)points.LongLength);
            if (success == -1)
                throw new NotImplementedException();
            else
            {
                if (!Inks.Contains(ink))
                {
                    Inks.Add(ink);
                }
                return success;
            }
        }

        public void RemoveAllInk()
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            Inks.Clear();
        }

        public void RemoveInk(List<PointF> ink)
        {
            // 先移除全部
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            if (!Inks.Contains(ink)) throw new NotImplementedException();
            // 再重新添加
            Inks.Remove(ink);
            foreach (var i in Inks)
            {
                AddInk(i);
            }
        }

        private void GetInks()
        {
            var count = fpdf_annot.FPDFAnnotGetInkListCount(Annotation);
            if (count == 0)
                throw new NotImplementedException("No ink at this annot");
            inks = new List<List<PointF>>();
            for (int index = 0; index < count; index++)
            {
                var pointCount = fpdf_annot.FPDFAnnotGetInkListPath(Annotation, (uint)index, null, 0);
                FS_POINTF_[] points = new FS_POINTF_[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    points[i] = new FS_POINTF_();
                }
                var success = fpdf_annot.FPDFAnnotGetInkListPath(Annotation, (uint)index, points[0], pointCount) == pointCount;
                if (!success)
                    throw new NotImplementedException();
                var ink = new List<PointF>();
                foreach (var point in points)
                {
                    ink.Add(new PointF(point.X, point.Y));
                }
                inks.Add(ink);
            }
        }
    }
}
