using CoreGraphics;
using Foundation;
using System;

namespace ApplePDF.PdfKit
{
    public class PdfFont : IDisposable
    {
        public string Name { get; protected set; }

        public CGDataProvider FontData { get; protected set; }

        protected PdfFont()
        { }

        public PdfFont(PdfDocument doc, byte[] fontData)
        {
            FontData = new CGDataProvider(fontData);
        }

        public PdfFont(PdfDocument doc, string fontName)
        {
            Name = fontName;
        }

        /// <summary>
        /// 除<see cref="CGFont"/>不用size, 其它都需要
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public T GetFont<T>(float size = default) where T : class
        {
            if (typeof(T) == typeof(CGFont))
            {
                if (FontData != null)
                    return CGFont.CreateFromProvider(FontData) as T;
                else
                    return CGFont.CreateWithFontName(Name) as T;
            }
            else if (typeof(T) == typeof(CoreText.CTFont))
            {
                if (FontData != null)
                    return CGFont.CreateFromProvider(FontData).ToCTFont(size) as T;
                else
                    return new CoreText.CTFont(Name, size) as T;
            }
#if MACOS
            else if (typeof(T) == typeof(AppKit.NSFont))
            {
                if (FontData != null)
                    return AppKit.NSFont.FromCTFont(CGFont.CreateFromProvider(FontData).ToCTFont(size)) as T;
                else
                    return AppKit.NSFont.FromFontName(Name, size) as T;
            }
#else
            else if (typeof(T) == typeof(UIKit.UIFont))
            {
                if (FontData != null)
                    throw new NotImplementedException("没有找到从byte[]构建UIFont的方法.");
                else
                    return UIKit.UIFont.FromName(Name, size) as T;
            }
#endif
            return null;
        }

        public void Dispose()
        {
            FontData?.Dispose();
            FontData = null;
        }
    }

    public class PdfFont<T> : PdfFont where T : NSObject//: IPdfFont 
    {
        public T Font { get; private set; }

        public PdfFont(T platformFont)
        {
            Font = platformFont;
            if (Font is CGFont)
                Name = (Font as CGFont).FullName;
            if (Font is CoreText.CTFont)
                Name = (Font as CoreText.CTFont).FullName;
#if MACOS
            if (Font is AppKit.NSFont)
                Name = (Font as AppKit.NSFont).FontName;
#else
            if (Font is UIKit.UIFont)
                Name = (Font as UIKit.UIFont).Name;
#endif
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public new T1 GetFont<T1>(float size) where T1 : NSObject
        {
            //如果类型一样, 直接返回
            if (typeof(T1) == typeof(T))
            {
                return Font as T1;
            }

            //CGFont最可能是从byte[]获取的, 因此其FontName可能是没有的
            if (Font is CGFont)
            {
                if (typeof(T1) == typeof(CGFont))
                {
                    return Font as T1;
                }
                else if (typeof(T) == typeof(CoreText.CTFont))
                {
                    return (Font as CGFont).ToCTFont(size) as T1;
                }
#if MACOS
                else if (typeof(T1) == typeof(AppKit.NSFont))
                {
                    return AppKit.NSFont.FromCTFont((Font as CGFont).ToCTFont(size)) as T1;
                }
#else
                else if (typeof(T1) == typeof(UIKit.UIFont))
                {
                    return UIKit.UIFont.FromName(Name, size) as T1;
                }
#endif
            };

            //类型不一样的, 默认都从Name获取
            return base.GetFont<T1>(size);
        }

        public new void Dispose()
        {
            Font = null;
            base.Dispose();
        }
    }
}