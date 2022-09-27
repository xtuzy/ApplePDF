using ApplePDF.PdfKit;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Demo.Maui.Services
{
    internal class SaveService
    {
        public static void Save(SKBitmap bitmap, string fileName)
        {
            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream))
            {
                bitmap.Encode(wstream, SKEncodedImageFormat.Png, 300);
                byte[] data = memStream.ToArray();

                if (data == null)
                {
                    throw new Exception("Encode returned null");
                }
                else if (data.Length == 0)
                {
                    throw new Exception("Encode returned empty array");
                }
                else
                {
                    try
                    {
                        var path = FileSystem.Current.AppDataDirectory;
                        using (var fs = new FileStream(Path.Combine(path, fileName), FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex) { throw new Exception("Save fail"); }
                }
            }
        }

#if ANDROID
        public static void Save(PdfPage page, string fileName)
        {
            var pageSize = page.GetSize();
            var androidBitmap = Android.Graphics.Bitmap.CreateBitmap((int)pageSize.Width, (int)pageSize.Height, Android.Graphics.Bitmap.Config.Argb8888);
            ApplePDF.Maui.Extensions.PdfPageExtension.RenderPage(page, androidBitmap, (float)DeviceDisplay.MainDisplayInfo.Density
                , 0, 0, androidBitmap.Width, androidBitmap.Height, false);
            var filePath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "result.jpeg");
            ApplePDF.Maui.Extensions.PdfPageExtension.WriteBitmapToFile(filePath, androidBitmap, 100);
        }
#endif
    }
}
