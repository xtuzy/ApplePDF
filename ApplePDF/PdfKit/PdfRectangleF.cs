using System.Drawing;

namespace ApplePDF.PdfKit
{
    /// <summary>
    /// Pdf坐标系以左下角为原点,向上递增,向右递增
    /// </summary>
    public struct PdfRectangleF
    {
        public static readonly PdfRectangleF Empty;

        public PointF LeftTopPoint;
        public PointF RightBottomPoint;
        public static PdfRectangleF FromLTRB(float l, float t, float r, float b)
        {
            PdfRectangleF rect = new PdfRectangleF();
            rect.LeftTopPoint = new PointF(l,t);
            rect.RightBottomPoint = new PointF(r,b);
            return rect;
        }
        public float Left => LeftTopPoint.X <= RightBottomPoint.X ? LeftTopPoint.X : RightBottomPoint.X;
        public float Right => LeftTopPoint.X >= RightBottomPoint.X ? LeftTopPoint.X : RightBottomPoint.X;
        public float Top => LeftTopPoint.Y >= RightBottomPoint.Y ? LeftTopPoint.Y : RightBottomPoint.Y;
        public float Bottom => LeftTopPoint.Y <= RightBottomPoint.Y ? LeftTopPoint.Y : RightBottomPoint.Y;
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

        /// <include file='doc\Rectangle.uex' path='docs/doc[@for="Rectangle.operator=="]/*' />
        /// <devdoc>
        ///    <para>
        ///       Tests whether two <see cref='System.Drawing.Rectangle'/>
        ///       objects have equal location and size.
        ///    </para>
        /// </devdoc>
        public static bool operator ==(PdfRectangleF left, PdfRectangleF right)
        {
            return (left.Left == right.Left
                    && left.Top == right.Top
                    && left.Right == right.Right
                    && left.Bottom == right.Bottom);
        }

        /// <include file='doc\Rectangle.uex' path='docs/doc[@for="Rectangle.operator!="]/*' />
        /// <devdoc>
        ///    <para>
        ///       Tests whether two <see cref='System.Drawing.Rectangle'/>
        ///       objects differ in location or size.
        ///    </para>
        /// </devdoc>
        public static bool operator !=(PdfRectangleF left, PdfRectangleF right)
        {
            return !(left == right);
        }

        public PointF LTPoint => LeftTopPoint;
        public PointF RTPoint => new PointF(Right, Top);
        public PointF LBPoint => new PointF(Left, Bottom);
        public PointF RBPoint => RightBottomPoint;
    }
}
