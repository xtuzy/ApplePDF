
using ApplePDF.PdfKit;
using PDFiumCore;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Desktop.Windows.Extension
{
    public static class PdfPageExtension
    {
        private static readonly object @lock = new object();
        public static Bitmap RenderPageToPdfNativeBitmap(PdfPage page, float density = 2, int renderFlags = (int)RenderFlags.RenderAnnotations)
        {
            var bmp = new SKBitmap();

            //Skiasharp方法
            // pin the managed array so that the GC doesn't move it
            var bounds = page.GetSize();
            var rawBytes = page.GetImage(density, density,renderFlags);
            var gcHandle = GCHandle.Alloc(rawBytes, GCHandleType.Pinned);
            // install the pixels with the color type of the pixel data
            int width = (int)(bounds.Width * density);
            int height = (int)(bounds.Height * density);
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            bmp.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, delegate { gcHandle.Free(); }, null);

            return bmp.ToBitmap();
        }

        public static Bitmap RenderPageToBitmap(PdfPage page, float density = 2, int renderFlags = (int)RenderFlags.RenderAnnotations)
        {
            var size = page.GetSize();
            return GetImage(page, size.Width, size.Height, density, density, 0, renderFlags);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfDocument.cs#L525
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="rotate">Page orientation: 0 (normal) 1 (rotated 90 degrees clockwise) 2 (rotated 180 degrees) 3 (rotated 90 degrees counter-clockwise)</param>
        /// <param name="flags"> 0 for normal display, or combination of the Page Rendering flags defined above. With the FPDF_ANNOT flag, it renders all annotations that do not require user-interaction, which are all annotations except widget and popup annotations.</param>
        /// <returns>The rendered image.</returns>
        public static Bitmap GetImage(PdfPage page, float w, float h, float xScale, float yScale, int rotate, int flags)
        {
            /* if ((flags & RenderFlags.DisableImageAntialiasing) != 0)
             {
                 width = (int)(width * dpiX / 72);
                 height = (int)(height * dpiY / 72);
             }*/
            var width = (int)(w* xScale);//按照Xamarin.Essential实现Uwp的,在Windows上以96为标准
            var height = (int)(h * yScale);

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //bitmap.SetResolution(xScale * 96, yScale * 96);
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var pointer = data.Scan0;

            try
            {
                var handle = fpdfview.FPDFBitmapCreateEx(width, height, (int)FPDFBitmapFormat.BGRA, pointer, width * 4);
                try
                {
                    uint background = true ? 0xFFFFFFFF : 0x00FFFFFF;

                    fpdfview.FPDFBitmapFillRect(handle, 0, 0, width, height, background);

                    fpdfview.FPDF_RenderPageBitmap(handle, page.Page, 0, 0, width, height, (int)rotate, flags);
                }
                catch (Exception ex)
                {
                    throw new Exception("error rendering page");
                }
                finally
                {
                    fpdfview.FPDFBitmapDestroy(handle);
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        public static Bitmap RenderPageBySKBitmap(PdfPage page, float density = 2,int renderFlags=(int)RenderFlags.RenderAnnotations)
        {
            SKBitmap bmp;

            //Skiasharp方法
            // pin the managed array so that the GC doesn't move it
            var bounds = page.GetSize();
            int width = (int)(bounds.Width * density);
            int height = (int)(bounds.Height * density);
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            bmp = new SKBitmap(info);

            page.GetImage(bmp.GetPixels(), density, density, 0,renderFlags);

            return bmp.ToBitmap();
        }
    }
}
