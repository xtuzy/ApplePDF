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
        const string TAG = nameof(PdfPagePathObj);

        internal PdfPagePathObj(FpdfPageobjectT pageObj) : base(pageObj, PdfPageObjectTypeFlag.PATH)
        {
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        /// <param name="pathList"></param>
        /// <param name="color"></param>
        /// <param name="pathWidth"></param>
        public void SetPath(List<PdfSegmentPath> pathList)
        {
            foreach (var path in pathList)
            {
                switch (path.Type)
                {
                    case PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN:
                        break;
                    case PdfSegmentFlag.FPDF_SEGMENT_LINETO:
                        if (fpdf_edit.FPDFPathLineTo(PageObj, path.Position.X, path.Position.Y) == 0)
                            Debug.WriteLine($"{TAG}:Fail set LineTo to path obj of annot");
                        break;
                    case PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO:
                        var tempPath = path as PdfBezierSegmentPath;
                        if (tempPath != null)
                            if (fpdf_edit.FPDFPathBezierTo(PageObj, tempPath.ControlPoint1.X, tempPath.ControlPoint1.Y, tempPath.ControlPoint2.X, tempPath.ControlPoint2.Y, tempPath.Position.X, tempPath.Position.Y) == 0)
                                Debug.WriteLine($"{TAG}:Fail set BezierTo to path obj of annot");
                        break;
                    case PdfSegmentFlag.FPDF_SEGMENT_MOVETO:
                        if (fpdf_edit.FPDFPathMoveTo(PageObj, path.Position.X, path.Position.Y) == 0)
                            Debug.WriteLine($"{TAG}:Fail set MoveTo to path obj of annot");
                        break;
                }
            }
        }

        public List<PdfSegmentPath> GetPath()
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
                        path.Type = PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO;
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
                                path.ControlPoint1 = new PointF(x, y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");

                            if (fpdf_edit.FPDFPathSegmentGetPoint(segment1, ref x, ref y) == 1)
                                path.ControlPoint2 = new PointF(x, y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");

                            if (fpdf_edit.FPDFPathSegmentGetPoint(segment1, ref x, ref y) == 1)
                                path.Position = new PointF(x, y);
                            else
                                throw new Exception("Fail to get BEZIER segment, because can't get right control points");
                            paths.Add(path);
                            segmentIndex = segmentIndex + 2;
                        }
                        else
                            throw new Exception("Fail to get BEZIER segment, because can't get right control points");
                    }
                    else
                    {
                        var path = new PdfSegmentPath();
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_MOVETO)
                            path.Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO;
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_LINETO)
                            path.Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO;
                        if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN)
                            path.Type = PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN;
                        if (fpdf_edit.FPDFPathSegmentGetPoint(segment, ref x, ref y) == 1)
                        {
                            path.Position = new PointF(x, y);
                            paths.Add(path);
                        }
                    }
                }
            }
            return paths;
        }

        public void SetStrokeWidth(float pathWidth)
        {
            if (fpdf_edit.FPDFPageObjSetStrokeWidth(PageObj, pathWidth) == 0)
                Debug.WriteLine($"{TAG}:Fail set StrokeWidth to PageObj of annot");
        }

        public void SetDrawMode(bool useStrokeMode = true)
        {
            if (useStrokeMode)
            {
                if (fpdf_edit.FPDFPathSetDrawMode(PageObj, (int)PdfFillMode.FPDF_FILLMODE_NONE, 1) == 0)
                    Debug.WriteLine($"{TAG}:Fail set DrawMode to PageObj of annot");
            }
            else
            {
                if (fpdf_edit.FPDFPathSetDrawMode(PageObj, (int)PdfFillMode.FPDF_FILLMODE_NONE, 0) == 0)
                    Debug.WriteLine($"{TAG}:Fail set DrawMode to PageObj of annot");
            }
        }

        /// <summary>
        /// return value see <see cref="PdfLineCap"/>. Maybe linecap have more type, so i return int.
        /// </summary>
        /// <returns>fail when return -1</returns>
        public int GetLineCap()
        {
            return fpdf_edit.FPDFPageObjGetLineCap(PageObj);
        }

        public bool SetLineCap(PdfLineCap cap)
        {
            return fpdf_edit.FPDFPageObjSetLineCap(PageObj, (int)cap) == 1;
        }

        public PdfLineJoin GetLineJoin()
        {
            return (PdfLineJoin)fpdf_edit.FPDFPageObjGetLineJoin(PageObj);
        }

        public bool GetLineJoin(PdfLineJoin join)
        {
            return fpdf_edit.FPDFPageObjSetLineJoin(PageObj, (int)join) == 1;
        }

        public static PdfPagePathObj Create(PointF startPoint)
        {
            //自己新建的需要标记tag,因为没有添加到pdf就需要主动释放资源
            return new PdfPagePathObj(fpdf_edit.FPDFPageObjCreateNewPath(startPoint.X, startPoint.Y)) { PageObjTag = 1 };
        }
    }

    public class PdfPageImageObj : PdfPageObj
    {
        const string TAG = nameof(PdfPageImageObj);
        internal PdfPageImageObj(FpdfPageobjectT pageObj) : base(pageObj, PdfPageObjectTypeFlag.IMAGE)
        {
        }

        public void SetImage(PdfPage page, FpdfBitmapT bitmap)
        {
            if (fpdf_edit.FPDFImageObjSetBitmap(page.Page, page.PageIndex, PageObj, bitmap) == 0)
            {
                Debug.WriteLine($"{TAG}:Fail to set bitmap to imageObj");
            }
        }

        public void SetImage(PdfPage page, IntPtr imageBuffer, Size imageSize)
        {
            // 我的理解是,这个方法让Pdfium的Bitmap对象的指针指向现成的图片,从而可以直接把图片写到Pdf
            var bitmap = fpdfview.FPDFBitmapCreateEx(imageSize.Width, imageSize.Height, (int)FPDFBitmapFormat.BGRA, imageBuffer, imageSize.Width * 4);
            if (bitmap == null)
            {
                throw new Exception($"{TAG}:Failed to create a pdf bitmap");
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
        internal PdfPageTextObj(FpdfPageobjectT pageObj) : base(pageObj, PdfPageObjectTypeFlag.TEXT)
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
    }
}
