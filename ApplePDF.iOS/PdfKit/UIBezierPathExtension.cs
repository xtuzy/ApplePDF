using ApplePDF.Extensions;
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit
{
    public static class UIBezierPathExtension
    {
#if IOS || MACCATALYST
        public static List<PdfSegmentPath> GetPath(UIKit.UIBezierPath bezierPath)
        {
            var cgPath = bezierPath.CGPath;
            var list = new List<PdfSegmentPath>();
            cgPath.Apply((element) =>
            {
                if (element.Type == CoreGraphics.CGPathElementType.MoveToPoint)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.MoveTo, Position = element.Point1.ToPointF() });
                if (element.Type == CoreGraphics.CGPathElementType.AddLineToPoint)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.LineTo, Position = element.Point1.ToPointF() });
                if (element.Type == CoreGraphics.CGPathElementType.AddCurveToPoint)
                    list.Add(new PdfBezierSegmentPath() {
                        Type = PdfSegmentPath.SegmentFlag.BezierTo,
                        ControlPoint1 = element.Point1.ToPointF(),
                        ControlPoint2 = element.Point2.ToPointF(),
                        Position = element.Point3.ToPointF()
                    });
                if (element.Type == CoreGraphics.CGPathElementType.CloseSubpath)
                    list[list.Count - 1].IsCloseToStart = true;
            });
            return list;
        }
#else
        public static List<PdfSegmentPath> GetPath(AppKit.NSBezierPath bezierPath)
        {
            if (bezierPath.ElementCount == 0) return null;
            var list = new List<PdfSegmentPath>();
            CGPoint[] tempPoint = new CGPoint[3];
            for(var i = 0; i < bezierPath.ElementCount - 1; i++) 
            {
                var elementType = bezierPath.ElementAt(i, out tempPoint);
                if (elementType == AppKit.NSBezierPathElement.MoveTo)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.MoveTo, Position = tempPoint[0].ToPointF() });
                if (elementType == AppKit.NSBezierPathElement.LineTo)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.LineTo, Position = tempPoint[0].ToPointF() });
                if (elementType == AppKit.NSBezierPathElement.CurveTo)
                    list.Add(new PdfBezierSegmentPath() { Type = PdfSegmentPath.SegmentFlag.BezierTo, ControlPoint1 = tempPoint[0].ToPointF(), ControlPoint2 = tempPoint[1].ToPointF(), Position = tempPoint[2].ToPointF() });
                if (elementType == AppKit.NSBezierPathElement.ClosePath)
                    list[list.Count - 1].IsCloseToStart = true;
            }
            return list;
        }
#endif
    }
}
