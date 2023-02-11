using CoreGraphics;
using CoreImage;
using System.Drawing;
#if MACOS
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
namespace ApplePDF.Extensions
{
    internal static class ColorExtension
    {
        public static Color ToColor(this CGColor color)
        {
            //var cicolor = new CIColor(color);//0-1
            //return Color.FromArgb((int)(cicolor.Alpha * 255), (int)(cicolor.Red * 255), (int)(cicolor.Green * 255), (int)(cicolor.Blue * 255));
            var uicolor = UIColor.FromCGColor(color);
            return uicolor.ToColor();
        }
  
        public static Color ToColor(this UIColor uicolor)
        {
#if MACOS
            uicolor.GetRgba(out var r, out var g, out var b, out var a);
#else
            uicolor.GetRGBA(out var r, out var g, out var b, out var a);
#endif
            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        public static CGColor ToCGColor(this Color color)
        {
#if MACOS
            var uicolor = UIColor.FromRgba(color.R, color.G, color.B, color.A);
#else
            var uicolor = UIColor.FromRGBA(color.R, color.G, color.B, color.A);
#endif
            return uicolor.CGColor;
        }

        public static UIColor ToUIColor(this Color color)
        {
#if MACOS
            return UIColor.FromRgba(color.R, color.G, color.B, color.A);
#else
            return UIColor.FromRGBA(color.R, color.G, color.B, color.A);
#endif
        }
    }
}
