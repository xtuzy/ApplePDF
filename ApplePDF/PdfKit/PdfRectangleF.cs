using System.Drawing;

namespace ApplePDF.PdfKit
{
    /// <summary>
    /// Pdf坐标系以左下角为原点,向上递增,向右递增
    /// </summary>
    public struct PdfRectangleF
    {
        public static readonly PdfRectangleF Empty;

        public PointF Point1;
        public PointF Point2;
        public static PdfRectangleF FromLTRB(float l, float t, float r, float b)
        {
            PdfRectangleF rect = new PdfRectangleF();
            rect.Point1 = new PointF(l,t);
            rect.Point2 = new PointF(r,b);
            return rect;
        }
        public float Left => Point1.X <= Point2.X ? Point1.X : Point2.X;
        public float Right => Point1.X >= Point2.X ? Point1.X : Point2.X;
        public float Top => Point1.Y >= Point2.Y ? Point1.Y : Point2.Y;
        public float Bottom => Point1.Y <= Point2.Y ? Point1.Y : Point2.Y;
        public float Width => Right - Left;
        public float Height => Top - Bottom;
        public bool IsContainPoint(PointF point)
        {
            if (Left <= point.X && Right >= point.X && Top >= point.Y && Bottom <= point.Y)
                return true;
            else
                return false;
        }

        public bool IsContainPoint(float x1, float y1)
        {
            return IsContainPoint(new PointF(x1, y1));
        }
    }
}
