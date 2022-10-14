using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Ink的生成有两种,一种使用<see cref="InkPointPaths"/>和<see cref="AnnotColor"/>,
    /// 另一种使用<see cref="PdfPagePathObj"/>, 建议使用后者
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation_CanWritePdfPageObj, IColorAnnotation
    {
        public PdfInkAnnotation() : base(PdfAnnotationSubtype.Ink)
        {
        }

        internal PdfInkAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
            AnnotColor = GetAnnotColor();

            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var pdfPageObjs = new List<PdfPageObj>();
                PdfPageObjs = pdfPageObjs;
                for (int objIndex = 0; objIndex < objectCount; objIndex++)
                {
                    var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                    if (obj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                        if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            pdfPageObjs.Add(new PdfPagePathObj(obj));
                        }
                    }
                }
            }
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

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);

            //添加时使用PdfPageObj优先级高,其可以控制更多细节
            if (PdfPageObjs != null)
            {
                foreach (PdfPageObj obj in PdfPageObjs)
                    AppendObjToAnnot(obj);
            }
            else
            {
                foreach (var i in InkPointPaths)
                {
                    AddInkPoints(i);
                }
                SetAnnotColor(AnnotColor);
            }
        }

        #region 使用InkList的Api

        List<List<PointF>> inks;
        public List<List<PointF>> InkPointPaths
        {
            get { if (inks == null) GetInkPoints(); return inks; }
            set { inks = value; }
        }

        /// <summary>
        /// Ink的颜色我在测试的Pdf上是通过StrokeColor获取的,但看Pdfium的测试使用的是这个
        /// </summary>
        public Color? AnnotColor { get; set; }

        /// <summary>
        /// 设置新的<see cref="AnnotColor"/>后,调用此方法更新
        /// </summary>
        public void UpdateInkPointPathsAnnotColor()
        {
            SetAnnotColor(AnnotColor);
        }

        public int AddInkPoints(List<PointF> ink)
        {
            FS_POINTF_[] points = new FS_POINTF_[ink.Count];
            for (var index = 0; index < ink.Count; index++)
            {
                points[index] = new FS_POINTF_() { X = ink[index].X, Y = ink[index].Y };
            }
            var success = fpdf_annot.FPDFAnnotAddInkStroke(Annotation, points[0], (ulong)points.LongLength);
            if (success == -1)
                throw new NotImplementedException("Add InkStroke fail");
            else
            {
                if (!InkPointPaths.Contains(ink))
                {
                    InkPointPaths.Add(ink);
                }
                return success;
            }
        }

        public void RemoveAllInk()
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            InkPointPaths.Clear();
        }

        public void RemoveInkPoints(List<PointF> ink)
        {
            // 先移除全部
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            if (!InkPointPaths.Contains(ink)) throw new NotImplementedException();
            // 再重新添加
            InkPointPaths.Remove(ink);
            foreach (var i in InkPointPaths)
            {
                AddInkPoints(i);
            }
        }

        private void GetInkPoints()
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
                float minLimit = 0.001f;//读取的很小的值可能有问题,直接筛掉
                foreach (var point in points)
                {
                    ink.Add(new PointF(point.X > minLimit ? point.X : 0f, point.Y > minLimit ? point.Y : 0f));
                }
                inks.Add(ink); fpdf_annot.FPDFAnnotAddInkStroke
            }
        }

        #endregion
    }
}
