using CoreGraphics;
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
#if IOS || MACCATALYST
        public static Color ToColor(this CGColor color)
        {
            var uicolor = UIColor.FromCGColor(color);
            uicolor.GetRGBA(out var r, out var g, out var b, out var a);
            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }
#endif    
        public static Color ToColor(this UIColor uicolor)
        {
#if MACOS
            uicolor.GetRgba(out var r, out var g, out var b, out var a);
#else
            uicolor.GetRGBA(out var r, out var g, out var b, out var a);
#endif
            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }
#if IOS || MACCATALYST
        public static CGColor ToCGColor(this Color color)
        {          
            var uicolor = UIColor.FromRGBA(color.R, color.G, color.B, color.A);
            return uicolor.CGColor;
        }
#endif
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
