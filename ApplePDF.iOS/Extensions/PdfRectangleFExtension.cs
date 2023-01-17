using ApplePDF.PdfKit;
using CoreGraphics;

namespace ApplePDF.Extensions
{
    internal static class PdfRectangleFExtension
    {
        public static CGRect ToCGRect(this PdfRectangleF rect)
        {
            return new CGRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static PdfRectangleF ToPdfRectangleF(this CGRect rect)
        {
            return PdfRectangleF.FromLTRB((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        }
    }
}
