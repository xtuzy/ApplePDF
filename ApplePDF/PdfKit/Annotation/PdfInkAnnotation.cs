using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Ink的生成有两种,一种使用<see cref="InkListPaths"/>和<see cref="AnnotColor"/>,
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
                if (InkListPaths != null)
                {
                    foreach (var i in InkListPaths)
                    {
                        AddInkPoints(i);
                    }
                }
                SetAnnotColor(AnnotColor);
            }
        }

        #region 使用InkList的Api

        List<List<PointF>> inks;
        public List<List<PointF>> InkListPaths
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
        public void UpdateInkListPathsAnnotColor()
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
                if (!InkListPaths.Contains(ink))
                {
                    InkListPaths.Add(ink);
                }
                return success;
            }
        }

        public void RemoveAllInk()
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            InkListPaths.Clear();
        }

        public void RemoveInkPoints(List<PointF> ink)
        {
            // 先移除全部
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            if (!InkListPaths.Contains(ink)) throw new NotImplementedException();
            // 再重新添加
            InkListPaths.Remove(ink);
            foreach (var i in InkListPaths)
            {
                AddInkPoints(i);
            }
        }

        private bool GetInkPoints()
        {
            var count = fpdf_annot.FPDFAnnotGetInkListCount(Annotation);
            if (count != 0)
            {
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
                    {
                        Debug.WriteLine("Fail to get ink points, because FPDFAnnotGetInkListPath return 0");
                        return false;
                    }
                    var ink = new List<PointF>();
                    float minLimit = 0.001f;//读取的很小的值可能有问题,直接筛掉
                    foreach (var point in points)
                    {
                        ink.Add(new PointF(point.X > minLimit ? point.X : 0f, point.Y > minLimit ? point.Y : 0f));
                    }
                    inks.Add(ink);
                }
                return true;
            }
            else
            {
                Debug.WriteLine("Fail to get ink points, because no ink at this annot");
                return false;
            }
        }

        #endregion
    }
}
