using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfSegmentPath
    {
        public PdfSegmentFlag Type;
        /// <summary>
        /// If is <see cref="PdfBezierSegmentPath"/>, this is End point of Bezier.
        /// </summary>
        public PointF Position;
    }

    /// <summary>
    /// For <see cref="fpdf_edit.FPDFPathBezierTo"/>
    /// </summary>
    public class PdfBezierSegmentPath: PdfSegmentPath
    {
        public PointF ControlPoint1;
        public PointF ControlPoint2;
    }
}
