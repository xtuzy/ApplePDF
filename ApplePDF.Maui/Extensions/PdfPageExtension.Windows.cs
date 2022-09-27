#if WINDOWS
using ApplePDF.PdfKit;
using PDFiumCore;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Math = System.Math;

namespace ApplePDF.Maui.Extensions
{
    public static class PdfPageExtension
    {
        public static Bitmap Draw(this PdfPage page, int width, int height, float dpiX, float dpiY, RenderFlags flags)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(dpiX, dpiY);

            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            try
            {
                var handle = fpdfview.FPDFBitmapCreateEx(width, height, 4, data.Scan0, width * 4);

                try
                {
                    fpdfview.FPDF_RenderPageBitmap(handle, page.Page, 0, 0, width, height, 0, (int)flags);
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

        public static void WriteBitmapToFile(string filePath, Bitmap bitmap, int quality)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                bitmap.Save(stream, ImageFormat.Png);
            }
        }
    }
}
#endif