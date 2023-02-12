using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Ink的生成有两种,一种使用<see cref="InkListPaths"/>和<see cref="AnnotColor"/>,
    /// 另一种使用<see cref="PdfPagePathObj"/>, 建议使用后者
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
        public Color? InkColor { get => GetAnnotColor(); set => SetAnnotColor(value); }

        public bool AddInkPoints(List<PointF> ink, bool defaultWay = false)
        {
            //Pdfium有直接设置Pointde的方法, 但测试时发现只有写入的第一个点是正确的,其余为0
            bool SetInkPointsDefaultInPdfium(List<PointF> ink)
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
                    return true;
                }
            }

            //这里设置string不对, 但没有设置数组的api
            bool SetInkPointsUseKey(List<PointF> ink)
            {
                var haveOldInk = fpdf_annot.FPDFAnnotHasKey(Annotation, PdfInkAnnotation.Constant.CommonKey.kInkList) == 1;
                StringBuilder resultStr = new StringBuilder();
                if( haveOldInk)
                {
                    //应该是[[x1,y1,x2,y2]]
                    var oldPointsListStr = GetStringValueFromKey(PdfInkAnnotation.Constant.CommonKey.kInkList);
                    resultStr.Append(oldPointsListStr, 0, oldPointsListStr.Length -1);
                }else
                    resultStr.Append("[");
                resultStr.Append("[");
                foreach(var point in ink)
                {
                    resultStr.Append($" {point.X.ToString("0.0000")} {point.Y.ToString("0.0000")}");
                }
                resultStr.Append("]]");
                return SetStringValueForKey(resultStr.ToString(), PdfInkAnnotation.Constant.CommonKey.kInkList);
            }

            if (defaultWay)
                return SetInkPointsDefaultInPdfium(ink);
            else
            {
                throw new NotImplementedException();
                return SetInkPointsUseKey(ink);
            }
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
