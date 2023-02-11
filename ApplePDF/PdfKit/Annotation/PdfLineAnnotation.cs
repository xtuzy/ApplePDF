
using PDFiumCore;
using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Pdfium的Line注释坐标有两种方式, 一种直接获取, 但无直接写入的Api(可以通过key), 一种通过页面对象, 但复杂
    /// </summary>
    public class PdfLineAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        public class Constant
        {
            public class CommonKey
            {
                /// <summary>
                /// 线首尾位置数组
                /// </summary>
                public const string kL = "L";
            }
        }

        internal PdfLineAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        public Color?  StrokeColor { get => GetAnnotColor(); set => SetAnnotColor(value); }

        public (PointF, PointF)? Location 
        { 
            get
            {
                //Location
                FS_POINTF_ start = new FS_POINTF_();
                FS_POINTF_ end = new FS_POINTF_();
                var success = fpdf_annot.FPDFAnnotGetLine(Annotation, start, end) == 1;
                if (success)
                {
                    var startLocation = new PointF(start.X, start.Y);
                    var endLocation = new PointF(end.X, end.Y);
                    return (startLocation, endLocation);
                }
                return null;
            }
            set
            {
                SetStringValueForKey($"[{value.Value.Item1.X} {value.Value.Item1.Y} {value.Value.Item2.X} {value.Value.Item2.Y}]", PdfLineAnnotation.Constant.CommonKey.kL);
            }
        }
    }
}
