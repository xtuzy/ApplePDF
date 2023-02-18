using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// pdf有inklist key存储点，但在ios中通过inklist key和path获得的结果都是path，因此iOS上请直接使用path，pdfium上
    /// 先尝试获取<see cref="PdfPagePathObj"/>，如果没有再获取inkpoints
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation_CanWritePdfPageObj
    {
        public class Constant
        {
            public class CommonKey
            {
                public const string kInkList = "InkList";
            }
        }

        internal PdfInkAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
        }

        #region 使用Ink Points的Api

        /// <summary>
        /// Ink的颜色我在测试的Pdf上是通过PageObj的StrokeColor获取的,但看Pdfium的测试使用的是这个
        /// </summary>
        public Color? InkPointsColor { get => GetAnnotColor(); set => SetAnnotColor(value); }

        public bool AddInkPoints(List<PointF> ink)
        {
            bool SetInkPointsDefaultInPdfium(List<PointF> ink)
            {
                fpdf_annot.FS_POINTF_Fix[] points = new fpdf_annot.FS_POINTF_Fix[ink.Count];
                for (var index = 0; index < ink.Count; index++)
                {
                    points[index] = new fpdf_annot.FS_POINTF_Fix() { X = ink[index].X, Y = ink[index].Y };
                }

                var success = fpdf_annot.FPDFAnnotAddInkStrokeTryFix(Annotation, ref points, (ulong)points.Length);
                if (success == -1)
                    throw new NotImplementedException("Add InkStroke fail");
                else
                {
                    return true;
                }
            }
            return SetInkPointsDefaultInPdfium(ink);
        }

        public void RemoveAllInkPoints()
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
        }

        public void RemoveInkPoints(List<PointF> ink)
        {
            // 先保存全部
            var inkListPaths = GetInkPoints();
            // 再从pdfium移除全部
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            if (!inkListPaths.Contains(ink)) throw new NotImplementedException();
            // 再重新添加
            inkListPaths.Remove(ink);
            foreach (var i in inkListPaths)
            {
                AddInkPoints(i);
            }
        }

        public List<List<PointF>> GetInkPoints()
        {
            var count = fpdf_annot.FPDFAnnotGetInkListCount(Annotation);
            if (count != 0)
            {
                var inks = new List<List<PointF>>();
                for (int index = 0; index < count; index++)
                {
                    fpdf_annot.FS_POINTF_Fix[] points = new fpdf_annot.FS_POINTF_Fix[0];
                    var pointCount = fpdf_annot.FPDFAnnotGetInkListPathTryFix(Annotation, (uint)index, ref points, 0);
                    points = new fpdf_annot.FS_POINTF_Fix[pointCount];
                    var success = fpdf_annot.FPDFAnnotGetInkListPathTryFix(Annotation, (uint)index, ref points, pointCount) == pointCount;
                    if (!success)
                    {
                        Debug.WriteLine("Fail to get ink points, because FPDFAnnotGetInkListPath return 0");
                        return null;
                    }
                    var ink = new List<PointF>();
                    float minLimit = 0.001f;//读取的很小的值可能有问题,直接筛掉
                    foreach (var point in points)
                    {
                        ink.Add(new PointF(point.X > minLimit ? point.X : 0f, point.Y > minLimit ? point.Y : 0f));
                    }
                    inks.Add(ink);
                }
                return inks;
            }
            else
            {
                Debug.WriteLine("Fail to get ink points, because no ink at this annot");
                return null;
            }
        }

        #endregion
    }
}
