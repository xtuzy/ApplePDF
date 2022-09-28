
using ApplePDF.PdfKit;
using PDFiumCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Demo.Maui.Extension
{
    public static class PdfPageExtension
    {
        private static readonly object @lock = new object();
        public static SKBitmap RenderPageToSKBitmapFormNativeImage(PdfPage page, float density = 2, int renderFlags = (int)RenderFlags.RenderAnnotations)
        {
            var bmp = new SKBitmap();

            //Skiasharp方法
            // pin the managed array so that the GC doesn't move it
            var bounds = page.GetSize();
            var rawBytes = page.Draw(density, density, renderFlags);
            var gcHandle = GCHandle.Alloc(rawBytes, GCHandleType.Pinned);
            // install the pixels with the color type of the pixel data
            int width = (int)(bounds.Width * density);
            int height = (int)(bounds.Height * density);
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            bmp.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, delegate { gcHandle.Free(); }, null);

            return bmp;
        }

        public static Stream RenderPageToStreamFormNativeImage(PdfPage page, float density = 2, int renderFlags = (int)RenderFlags.RenderAnnotations)
        {
            // pin the managed array so that the GC doesn't move it
            var bounds = page.GetSize();
            var rawBytes = page.Draw(density, density, renderFlags);
            var gcHandle = GCHandle.Alloc(rawBytes, GCHandleType.Pinned);
            var memoryStream = new MemoryStream(rawBytes);
            return memoryStream;
        }

        public static SKBitmap RenderPageToSKBitmapFormSKBitmap(PdfPage page, float density = 2, int renderFlags = (int)RenderFlags.RenderAnnotations)
        {
            SKBitmap bmp;

            //Skiasharp方法
            // pin the managed array so that the GC doesn't move it
            var bounds = page.GetSize();
            int width = (int)(bounds.Width * density);
            int height = (int)(bounds.Height * density);
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            bmp = new SKBitmap(info);

            page.Draw(bmp.GetPixels(), density, density, 0, renderFlags);

            return bmp;
        }

        public static MemoryStream SKBitmapToStream(this SKBitmap bmp)
        {
            var stream = new MemoryStream();
            bmp.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Position = 0;
            return stream;
        }
    }
}
