using ApplePDF.Extensions;
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                else if (element.Type == CoreGraphics.CGPathElementType.AddLineToPoint)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.LineTo, Position = element.Point1.ToPointF() });
                else if (element.Type == CoreGraphics.CGPathElementType.AddCurveToPoint)
                    list.Add(new PdfBezierSegmentPath() {
                        Type = PdfSegmentPath.SegmentFlag.BezierTo,
                        ControlPoint1 = element.Point1.ToPointF(),
                        ControlPoint2 = element.Point2.ToPointF(),
                        Position = element.Point3.ToPointF()
                    });
                else if (element.Type == CoreGraphics.CGPathElementType.CloseSubpath)
                    list[list.Count - 1].IsCloseToStart = true;
            });
            return list;
        }
        
        public static void AddPath(this UIKit.UIBezierPath bezierPath, List<PdfSegmentPath> pathList)
        {
            foreach (var path in pathList)
            {
                switch (path.Type)
                {
                    case PdfSegmentPath.SegmentFlag.Unknow:
                        break;
                    case PdfSegmentPath.SegmentFlag.LineTo:
                        bezierPath.AddLineTo(path.Position.ToCGPoint());
                        break;
                    case PdfSegmentPath.SegmentFlag.BezierTo:
                        var tempPath = path as PdfBezierSegmentPath;
                        bezierPath.AddCurveToPoint(tempPath.Position, tempPath.ControlPoint1, tempPath.ControlPoint2);
                        break;
                    case PdfSegmentPath.SegmentFlag.MoveTo:
                        bezierPath.MoveTo(path.Position.ToCGPoint());
                        break;
                }
                if (path.IsCloseToStart)
                {
                    bezierPath.ClosePath();
                }
            }
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
                else if (elementType == AppKit.NSBezierPathElement.LineTo)
                    list.Add(new PdfSegmentPath() { Type = PdfSegmentPath.SegmentFlag.LineTo, Position = tempPoint[0].ToPointF() });
                else if (elementType == AppKit.NSBezierPathElement.CurveTo)
                    list.Add(new PdfBezierSegmentPath() { Type = PdfSegmentPath.SegmentFlag.BezierTo, ControlPoint1 = tempPoint[0].ToPointF(), ControlPoint2 = tempPoint[1].ToPointF(), Position = tempPoint[2].ToPointF() });
                else if (elementType == AppKit.NSBezierPathElement.ClosePath)
                    list[list.Count - 1].IsCloseToStart = true;
            }
            return list;
        }
#endif
    }
}
