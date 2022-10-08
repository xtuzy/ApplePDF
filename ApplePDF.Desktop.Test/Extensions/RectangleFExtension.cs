using System.Drawing;

namespace Pdf.Net.Test.Extensions
{
    /// <summary>
    /// Pdf坐标系以左下角为原点，System.Drawing以左上角
    /// </summary>
    internal static class RectangleFExtension
    {
        public static bool IsContainPoint(this RectangleF rect, PointF point)
        {
            if (rect.Left <= point.X && rect.Right >= point.X && rect.Top >= point.Y && rect.Bottom <= point.Y)
                return true;
            else
                return false;
        }

        public static bool IsContainPoint(this RectangleF rect, float x1, float y1)
        {
            return IsContainPoint(rect, new PointF(x1, y1));
        }
    }
}
