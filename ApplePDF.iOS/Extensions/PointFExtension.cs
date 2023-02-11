﻿using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Extensions
{
    internal static class PointFExtension
    {
        public static CGPoint ToCGPoint(this PointF point)
        {
            return new CGPoint(point.X, point.Y);
        }

        public static PointF ToPointF(this CGPoint point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }
    }

    internal static class PointExtension
    {
        public static CGPoint ToCGPoint(this Point point)
        {
            return new CGPoint(point.X, point.Y);
        }

        public static Point ToPoint(this CGPoint point)
        {
            return new Point((int)point.X, (int)point.Y);
        }
    }
}
