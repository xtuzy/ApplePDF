using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfPagePathObj : PdfPageObj
    {
        public enum PdfFillMode
        {
            None = PDFiumCore.PdfFillMode.FPDF_FILLMODE_NONE,
            Alternate = PDFiumCore.PdfFillMode.FPDF_FILLMODE_ALTERNATE,
            Winding = PDFiumCore.PdfFillMode.FPDF_FILLMODE_WINDING
        }

        public enum PdfLineCap
        {
            Butt = PDFiumCore.PdfLineCap.FPDF_LINECAP_BUTT,
            Round = PDFiumCore.PdfLineCap.FPDF_LINECAP_ROUND,
            Square = PDFiumCore.PdfLineCap.FPDF_LINECAP_PROJECTING_SQUARE,
        }

        public enum PdfLineJoin
        {
            Miter = PDFiumCore.PdfLineJoin.FPDF_LINEJOIN_MITER,
            Round = PDFiumCore.PdfLineJoin.FPDF_LINEJOIN_ROUND,
            Bevel = PDFiumCore.PdfLineJoin.FPDF_LINEJOIN_BEVEL,
        }

        const string TAG = nameof(PdfPagePathObj);

        public PdfPagePathObj(FpdfPageobjectT pageObj) : base(pageObj, TypeFlag.Path)
        {
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// Path中的位置应该是相对/Rect的值, Pdfium存储path时会计算成相对page的值，而iOS的是相对注释的，我们在外层适配iOS。
        /// 内部转化成Pdfium的实现， 因此需要指定注释的原点。
        /// </summary>
        /// <param name="pathList"></param>
        /// <param name="color"></param>
        /// <param name="pathWidth"></param>
        public void AddPath(List<PdfSegmentPath> pathList, PointF origrinPoint = default)
        {
            foreach (var path in pathList)
            {
                switch (path.Type)
                {
                    case PdfSegmentPath.SegmentFlag.Unknow:
                        break;
                    case PdfSegmentPath.SegmentFlag.LineTo:
                        var xL = path.Position.X + origrinPoint.X;
                        var yL = path.Position.Y + origrinPoint.Y;
                        if (fpdf_edit.FPDFPathLineTo(PageObj, xL, yL) == 0)
                            Debug.WriteLine($"{TAG}:Fail set LineTo to path obj of annot");
                        break;
                    case PdfSegmentPath.SegmentFlag.BezierTo:
                        var tempPath = path as PdfBezierSegmentPath;
                        if (tempPath != null)
                        {
                            var xB = tempPath.Position.X + origrinPoint.X;
                            var yB = tempPath.Position.Y + origrinPoint.Y;
                            var xC1B = tempPath.ControlPoint1.X + origrinPoint.X;
                            var yC1B = tempPath.ControlPoint1.Y + origrinPoint.Y;
                            var xC2B = tempPath.ControlPoint2.X + origrinPoint.X;
                            var yC2B = tempPath.ControlPoint2.Y + origrinPoint.Y;
                            if (fpdf_edit.FPDFPathBezierTo(PageObj, xC1B, yC1B, xC2B, yC2B, xB, yB) == 0)
                                Debug.WriteLine($"{TAG}:Fail set BezierTo to path obj of annot");
                        }
                        break;
                    case PdfSegmentPath.SegmentFlag.MoveTo:
                        var xM = path.Position.X + origrinPoint.X;
                        var yM = path.Position.Y + origrinPoint.Y;
                        if (fpdf_edit.FPDFPathMoveTo(PageObj, xM, yM) == 0)
                            Debug.WriteLine($"{TAG}:Fail set MoveTo to path obj of annot");
                        break;
                }
                if (path.IsCloseToStart)
                {
                    fpdf_edit.FPDFPathClose(PageObj);
                }
            }
        }

        /// <summary>
        /// Pdfium和iOS都根据/BBox绘图，iOS获取的Path是相对/BBox的，无论/BBox是0还是/Rect，但Pdfium获取不到/BBox值，所以我不知道/AP中坐标是相对谁的。
        /// 从 <see href="https://bugs.chromium.org/p/pdfium/issues/detail?id=1404&q=bbox&can=1"/>我们知道设置AP时需要设置/InkList，从其它软件写的注释中我也发现了这点。/InkList坐标代表了AP的Path。
        /// 而且确定相对Page，那么对照可以得出AP中是否是相对Page，相对Page，那就需要减去/Rect，获得相对/Rect的Path。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<PdfSegmentPath> GetPath(PointF origrinPoint = default)
        {
            var paths = new List<PdfSegmentPath>();
            var segmentCount = fpdf_edit.FPDFPathCountSegments(PageObj);
            float x = 0;
            float y = 0;
            for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
            {
                var segment = fpdf_edit.FPDFPathGetPathSegment(PageObj, segmentIndex);
                if (segment != null)
                {
                    if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO)
                    {
                        //如果是BEZIERTO,那么连续三个点都是BEZIERTO,匹配BEZIER的两个控制点和一个结束点
                        var path = new PdfBezierSegmentPath();
                        path.Type = PdfSegmentPath.SegmentFlag.BezierTo;
                        var segment1 = fpdf_edit.FPDFPathGetPathSegment(PageObj, segmentIndex + 1);
                        var segment2 = fpdf_edit.FPDFPathGetPathSegment(PageObj, segmentIndex + 2);
                        if (segment1 == null || segment2 == null)
                        {
                            throw new Exception("Fail to get BEZIER segment, because can't get right control points");
                        }
                        if (fpdf_edit.FPDFPathSegmentGetType(segment1) == (int)PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO
                            && fpdf_edit.FPDFPathSegmentGetType(segment2) == (int)PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO)
                        {
                            if (fpdf_edit.FPDFPathSegmentGetPoint(segment, ref x, ref y) == 1)
                                path.ControlPoint1 = new PointF(x - origrinPoint.X, y - origrinPoint.Y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");

                            if (fpdf_edit.FPDFPathSegmentGetPoint(segment1, ref x, ref y) == 1)
                                path.ControlPoint2 = new PointF(x - origrinPoint.X, y - origrinPoint.Y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");

                            if (fpdf_edit.FPDFPathSegmentGetPoint(segment2, ref x, ref y) == 1)
                                path.Position = new PointF(x - origrinPoint.X, y - origrinPoint.Y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");
                            paths.Add(path);
                            segmentIndex = segmentIndex + 2;
                        }
                        else
                            throw new Exception("Fail to get BEZIER segment, because can't get right control points");
                        if (fpdf_edit.FPDFPathSegmentGetClose(segment2) != 0)
                        {
                            path.IsCloseToStart = true;
                        }
                    }
                    else
                    {
                        var path = new PdfSegmentPath();
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_MOVETO)
                            path.Type = PdfSegmentPath.SegmentFlag.MoveTo;
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_LINETO)
                            path.Type = PdfSegmentPath.SegmentFlag.LineTo;
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN)
                            path.Type = PdfSegmentPath.SegmentFlag.Unknow;
                        if (fpdf_edit.FPDFPathSegmentGetPoint(segment, ref x, ref y) == 1)
                        {
                            if (fpdf_edit.FPDFPathSegmentGetClose(segment) != 0)
                            {
                                path.IsCloseToStart = true;
                            }
                            path.Position = new PointF(x - origrinPoint.X, y - origrinPoint.Y);
                            paths.Add(path);
                        }
                    }
                }
            }
            return paths;
        }

        public float? StrokeWidth
        {
            set
            {
                if(value.HasValue)
                    if (fpdf_edit.FPDFPageObjSetStrokeWidth(PageObj, value.Value) == 0)
                        throw new OperationCanceledException($"{TAG}:Fail set StrokeWidth to PageObj of annot");
            }

            get
            {
                float w = 0;
                if (fpdf_edit.FPDFPageObjGetStrokeWidth(PageObj, ref w) == 1)
                    return w;
                else
                    return null;
            }
        }

        public void SetDrawMode(bool useStrokeMode = true, PdfFillMode useFillMode = PdfFillMode.None)
        {
            if (fpdf_edit.FPDFPathSetDrawMode(PageObj, (int)useFillMode, useStrokeMode == true ? 1 : 0) == 0)
                throw new OperationCanceledException($"{TAG}:Fail set DrawMode to PageObj of annot");
        }

        public (bool useStrokeMode, PdfFillMode fillMode) GetDrawMode()
        {
            int useStrokeMode = -1;
            int fillMode = -1;
            if (fpdf_edit.FPDFPathGetDrawMode(PageObj, ref fillMode, ref useStrokeMode) == 0)
                throw new OperationCanceledException($"{TAG}:Fail set DrawMode to PageObj of annot");
            return (useStrokeMode == 1 ? true : false, (PdfFillMode)(fillMode));
        }

        /// <summary>
        /// return value see <see cref="PdfLineCap"/>. Maybe linecap have more type, so i return int.
        /// </summary>
        /// <returns>fail when return -1</returns>
        public PdfLineCap LineCap
        {
            get
            {
                return (PdfLineCap)fpdf_edit.FPDFPageObjGetLineCap(PageObj);
            }

            set
            {
                fpdf_edit.FPDFPageObjSetLineCap(PageObj, (int)value);
            }
        }

        public PdfLineJoin LineJoin
        {
            get
            {
                return (PdfLineJoin)fpdf_edit.FPDFPageObjGetLineJoin(PageObj);
            }

            set
            {
                fpdf_edit.FPDFPageObjSetLineJoin(PageObj, (int)value);
            }
        }

        public static PdfPagePathObj Create(PointF startPoint, bool generateApStream = true)
        {
            //自己新建的需要标记tag,因为没有添加到pdf就需要主动释放资源
            return new PdfPagePathObj(fpdf_edit.FPDFPageObjCreateNewPath(startPoint.X, startPoint.Y)) { PageObjTag = 1 };
        }

        /// <summary>
        /// Set obj will generate /AP, need set /InkList. <see cref="https://bugs.chromium.org/p/pdfium/issues/detail?id=1404&q=bbox&can=1"/>
        /// </summary>
        public List<PointF> GenerateInkPoints(List<PdfSegmentPath> path, PointF originalPoint)
        {
            List<PointF> points = new List<PointF>();
            foreach (var segment in path)
                points.Add(new PointF(segment.Position.X + originalPoint.X, segment.Position.Y + originalPoint.Y));
            return points;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class PdfPageImageObj : PdfPageObj
    {
        const string TAG = nameof(PdfPageImageObj);
        public PdfPageImageObj(FpdfPageobjectT pageObj) : base(pageObj, TypeFlag.Image)
        {
        }

        public void SetImage(PdfPage page, FpdfBitmapT bitmap)
        {
            if (fpdf_edit.FPDFImageObjSetBitmap(page.Page, page.PageIndex, PageObj, bitmap) == 0)
            {
                throw new OperationCanceledException($"{TAG}:Fail to set bitmap to imageObj");
            }
        }

        public void SetImage(PdfPage page, IntPtr imageBuffer, Size imageSize)
        {
            // 我的理解是,这个方法让Pdfium的Bitmap对象的指针指向现成的图片,从而可以直接把图片写到Pdf
            var bitmap = fpdfview.FPDFBitmapCreateEx(imageSize.Width, imageSize.Height, (int)FPDFBitmapFormat.BGRA, imageBuffer, imageSize.Width * 4);
            if (bitmap == null)
            {
                throw new OperationCanceledException($"{TAG}:Failed to create a pdf bitmap");
            }
            SetImage(page, bitmap);
        }

        public static PdfPageImageObj Create(PdfDocument doc)
        {
            return new PdfPageImageObj(fpdf_edit.FPDFPageObjNewImageObj(doc.Document)) { PageObjTag = 1 };
        }
    }

    public class PdfPageTextObj : PdfPageObj
    {
        const string TAG = nameof(PdfPageTextObj);
        public PdfPageTextObj(FpdfPageobjectT pageObj) : base(pageObj, TypeFlag.Text)
        {
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>Return false when fail to set text to text obj</returns>
        public bool SetText(string text)
        {
            // 设置文本
            // string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(text);
            ushort[] value = new ushort[text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);
            return fpdf_edit.FPDFTextSetText(PageObj, ref value[0]) == 1;
        }

        public string? GetText(PdfPage page)
        {
            // 尝试从文本对象获取Text
            var buffer = new ushort[100];
            var result = fpdf_edit.FPDFTextObjGetText(PageObj, page.TextPage, ref buffer[0], (uint)buffer.Length);
            if (result != 0)
            {
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                    {
                        return new string((char*)dataPtr, 0, (int)result);
                    }
                }
            }
            return null;
        }

        public string GetFontName()
        {
            //字体
            using (var font = new PdfFont(fpdf_edit.FPDFTextObjGetFont(PageObj)))
            {
                return font.Name;
            }
        }

        public float GetFontSize()
        {
            var textSize = 0f;
            if (fpdf_edit.FPDFTextObjGetFontSize(PageObj, ref textSize) == 0)
                Debug.WriteLine($"{TAG}:Fail to get font size");
            return textSize;
        }

        public static PdfPageTextObj Create(PdfDocument doc, string fontName, float fontSize)
        {
            return new PdfPageTextObj(fpdf_edit.FPDFPageObjNewTextObj(doc.Document, fontName, fontSize)) { PageObjTag = 1 };
        }

        public static PdfPageTextObj Create(PdfDocument doc, PdfFont font, float fontSize)
        {
            return new PdfPageTextObj(fpdf_edit.FPDFPageObjCreateTextObj(doc.Document, font.Font, fontSize)) { PageObjTag = 1 };
        }
    }

    public class PdfPageFormObj : PdfPageObj
    {
        const string TAG = nameof(PdfPageFormObj);
        public PdfPageFormObj(FpdfPageobjectT pageObj) : base(pageObj, TypeFlag.Form)
        {
            var count = fpdf_edit.FPDFFormObjCountObjects(PageObj);
            for (int i = 0; i < count; i++)
            {
                var obj = fpdf_edit.FPDFFormObjGetObject(PageObj, (uint)i);
                var type = (TypeFlag)fpdf_edit.FPDFPageObjGetType(obj);
            }
        }
    }
}
