using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfLineAnnotation : PdfAnnotation, IStrokeColorAnnotation
    {
        const string TAG = nameof(PdfLineAnnotation);
        public Color? StrokeColor { get; set; }

        List<List<PdfSegmentPath>> Paths;

        public PdfLineAnnotation() 
            : base(PdfAnnotationSubtype.Line)
        {
        }

        internal PdfLineAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            //Location
            FS_POINTF_ start = new FS_POINTF_();
            FS_POINTF_ end = new FS_POINTF_();
            bool success = false;
            success = fpdf_annot.FPDFAnnotGetLine(Annotation, start, end) == 1;
            if (success)
            {
                StartLocation = new PointF(start.X, start.Y);
                EndLocation = new PointF(end.X, end.Y);
            }

            // 颜色
            StrokeColor = GetFillAndStrokeColor().StrokeColor;

            //More line detail
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount >= 1)
            {
                Paths = new List<List<PdfSegmentPath>>();

                for (int i = 0; i < objectCount; i++)
                {
                    var paths = new List<PdfSegmentPath>();
                    Paths.Add(paths);

                    var lineObj = fpdf_annot.FPDFAnnotGetObject(Annotation, i);
                    if (lineObj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(lineObj);

                        if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            var segmentCount = fpdf_edit.FPDFPathCountSegments(lineObj);
                            if (segmentCount == -1)
                            {
                                throw new NotImplementedException($"{TAG}:Can't get segment count");
                            }

                            float x = 0;
                            float y = 0;
                            for (int j = 0; j < segmentCount; j++)
                            {
                                var path = new PdfSegmentPath();
                                paths.Add(path);
                                var segment = fpdf_edit.FPDFPathGetPathSegment(lineObj, j);
                                if (segment != null)
                                {
                                    if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_MOVETO)
                                        path.Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO;
                                    if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_LINETO)
                                        path.Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO;
                                    if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO)
                                        path.Type = PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO;
                                    if (fpdf_edit.FPDFPathSegmentGetType(segment) == (int)PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN)
                                        path.Type = PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN;
                                    fpdf_edit.FPDFPathSegmentGetPoint(segment, ref x, ref y);
                                    path.Position = new PointF(x, y);
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException($"The object at index of {i} is not path");
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Not only one object, don't know how to get correct object");
            }
        }

        public PointF StartLocation;
        public PointF EndLocation;
        public float StrokeWidth;

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            // 当前仅支持画直线
            var pathObj = fpdf_edit.FPDFPageObjCreateNewPath(StartLocation.X, StartLocation.Y);
            var success = fpdf_edit.FPDFPathLineTo(pathObj, EndLocation.X, EndLocation.Y) == 1;
            if (!success) throw new NotImplementedException($"{TAG}:Add line fail");
            success = fpdf_edit.FPDFPathClose(pathObj) == 1;
            if (!success) throw new NotImplementedException($"{TAG}:Close line fail");

            if (StrokeColor != null)
            {
                success = fpdf_edit.FPDFPageObjSetStrokeColor(pathObj, StrokeColor.Value.R, StrokeColor.Value.G, StrokeColor.Value.B, StrokeColor.Value.A) == 1;
                if (!success) throw new NotImplementedException($"{TAG}:Set stroke color fail");
            }

            success = fpdf_edit.FPDFPageObjSetStrokeWidth(pathObj, StrokeWidth) == 1;
            if (!success) throw new NotImplementedException($"{TAG}:Set line width fail");

            success = fpdf_annot.FPDFAnnotAppendObject(Annotation, pathObj) == 1;
            if (!success) throw new NotImplementedException($"{TAG}:Append line fail");
        }
    }
}
