#if ANDROID
using Android.Graphics;
using Android.Nfc;
using Android.Runtime;
using Android.Util;
using Android.Views;
using ApplePDF.PdfKit;
using Java.IO;
using Java.Lang;
using PDFiumCore;
using System;
using System.Runtime.InteropServices;
using static Android.App.ActionBar;
using static Android.Graphics.ColorSpace;
using static ApplePDF.Maui.Extensions.AndroidNative;
using Math = System.Math;

namespace ApplePDF.Maui.Extensions
{
    /// <summary>
    /// Android NDK的方法参考自<see cref="https://developer.android.com/ndk/reference/group/a-native-window"/>
    /// 和<see cref="http://android-doc.github.io/ndk/reference/group___native_activity.html"/>,后者直接提供了枚举的int值。
    /// 
    /// C#对接JNI的方式参考<see cref="https://github.com/luca-piccioni/OpenGL.Net/blob/master/OpenGL.Net.Xamarin.Android/GLSurfaceView.cs"/>
    /// </summary>
    static class AndroidNative
    {
        #region Native Activity
        //摘自http://android-doc.github.io/ndk/reference/group___native_activity.html
        internal static int WINDOW_FORMAT_RGBA_8888 = 1;
        internal static int WINDOW_FORMAT_RGBX_8888 = 2;
        internal static int WINDOW_FORMAT_RGB_565 = 4;
        #endregion

        /// <summary>
        /// 这段代码来自<see cref="https://github.com/luca-piccioni/OpenGL.Net/blob/master/OpenGL.Net.Xamarin.Android/GLSurfaceView.cs"/>
        /// Get native window from surface
        /// </summary>
        /// <param name="jni"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        [DllImport("android")]
        internal static extern IntPtr ANativeWindow_fromSurface(IntPtr jni, IntPtr surface);

        /// <summary>
        /// 这段代码来自<see cref="https://github.com/luca-piccioni/OpenGL.Net/blob/master/OpenGL.Net.Xamarin.Android/GLSurfaceView.cs"/>
        /// Get native window from surface
        /// </summary>
        /// <param name="surface"></param>
        [DllImport("android")]
        internal static extern void ANativeWindow_release(IntPtr surface);
        [DllImport("android")]
        internal static extern int ANativeWindow_getFormat(IntPtr window);
        [DllImport("android")]
        internal static extern void ANativeWindow_setBuffersGeometry(IntPtr window, int w, int h, int format);
        [DllImport("android")]
        internal static extern int ANativeWindow_getWidth(IntPtr window);
        [DllImport("android")]
        internal static extern int ANativeWindow_getHeight(IntPtr window);
        [DllImport("android")]
        internal static extern int ANativeWindow_lock(IntPtr window, out ANativeWindow_Buffer outBuffer, IntPtr inOutDirtyBounds);
        [DllImport("android")]
        internal static extern void ANativeWindow_unlockAndPost(IntPtr nativeWindow);

        /// <summary>
        /// <see cref="https://android.googlesource.com/platform/frameworks/native/+/android-o-mr1-preview-2/libs/nativewindow/include/android/native_window.h"/>
        /// </summary>
        internal struct ANativeWindow_Buffer
        {
            internal int width;
            internal int height;
            internal int stride;
            internal int format;
            internal IntPtr bits;
            internal uint[] reserved;
        }

        internal struct rgb
        {
            //uint8转C#参考https://stackoverflow.com/questions/24947123/does-c-sharp-have-int8-and-uint8
            //uint8_t red;
            internal sbyte red;
            //uint8_t green;
            internal sbyte green;
            //uint8_t blue;
            internal sbyte blue;
        };

    }

    /// <summary>
    /// 处理Pdfium对接Android的方法参考<see cref="https://github.com/barteksc/PdfiumAndroid/blob/master/src/main/java/com/shockwave/pdfium/PdfiumCore.java"/>
    /// 
    /// </summary>
    public static class PdfPageExtension
    {
        static object o = new object();

        public static void RenderPage(this PdfPage page, Bitmap bitmap, float systemDpi,
                                 int startX, int startY, int drawSizeX, int drawSizeY,
                                 bool renderAnnot)
        {
            lock (o)
            {
                try
                {
                    nativeRenderPageBitmap(page, bitmap, systemDpi,
                            startX, startY, drawSizeX, drawSizeY, renderAnnot);
                }
                catch (NullPointerException e)
                {
                    Log.Error(nameof(PdfPageExtension), "mContext may be null");
                    e.PrintStackTrace();
                }
                catch (Java.Lang.Exception e)
                {
                    Log.Error(nameof(PdfPageExtension), "Exception throw from native");
                    e.PrintStackTrace();
                }
            }
        }

        private unsafe static void nativeRenderPageBitmap(PdfPage page, Bitmap bitmap, float systemDpi,
                int startX, int startY, int drawSizeHor, int drawSizeVer, bool renderAnnot)
        {
            if (page == null || bitmap == null)
            {
                Log.Error(nameof(PdfPageExtension), "Render page pointers invalid");
                return;
            }

            AndroidBitmapInfo info = bitmap.GetBitmapInfo();

            //if (info.Format != ANDROID_BITMAP_FORMAT_RGBA_8888 && info.format != ANDROID_BITMAP_FORMAT_RGB_565)
            if (info.Format != Format.Rgba8888 && info.Format != Format.Rgb565)
            {
                Log.Error(nameof(PdfPageExtension), "Bitmap format must be RGBA_8888 or RGB_565");
                return;
            }
            IntPtr addr = bitmap.LockPixels();

            int canvasHorSize = (int)info.Width;
            int canvasVerSize = (int)info.Height;
            IntPtr tmp;
            int format;
            int sourceStride;
            if (info.Format == Format.Rgb565)
            {
                tmp = Marshal.AllocHGlobal(canvasVerSize * canvasHorSize * sizeof(AndroidNative.rgb));//https://stackoverflow.com/a/11711313
                sourceStride = canvasHorSize * sizeof(AndroidNative.rgb);
                format = (int)FPDFBitmapFormat.BGR;
            }
            else
            {
                tmp = addr;
                sourceStride = (int)info.Stride;
                format = (int)FPDFBitmapFormat.BGRA;
            }

            var pdfBitmap = fpdfview.FPDFBitmapCreateEx(canvasHorSize, canvasVerSize,
                                                             format, tmp, sourceStride);

            /*LOGD("Start X: %d", startX);
            LOGD("Start Y: %d", startY);
            LOGD("Canvas Hor: %d", canvasHorSize);
            LOGD("Canvas Ver: %d", canvasVerSize);
            LOGD("Draw Hor: %d", drawSizeHor);
            LOGD("Draw Ver: %d", drawSizeVer);*/

            if (drawSizeHor < canvasHorSize || drawSizeVer < canvasVerSize)
            {
                fpdfview.FPDFBitmapFillRect(pdfBitmap, 0, 0, canvasHorSize, canvasVerSize,
                                      0x848484FF); //Gray
            }

            int baseHorSize = (canvasHorSize < drawSizeHor) ? canvasHorSize : (int)drawSizeHor;
            int baseVerSize = (canvasVerSize < drawSizeVer) ? canvasVerSize : (int)drawSizeVer;
            int baseX = (startX < 0) ? 0 : (int)startX;
            int baseY = (startY < 0) ? 0 : (int)startY;
            int flags = (int)RenderFlags.FPDF_REVERSE_BYTE_ORDER;

            if (renderAnnot)
            {
                flags |= (int)RenderFlags.RenderAnnotations;
            }

            fpdfview.FPDFBitmapFillRect(pdfBitmap, baseX, baseY, baseHorSize, baseVerSize,
                                 0xFFFFFFFF); //White

            fpdfview.FPDF_RenderPageBitmap(pdfBitmap, page.Page,
                                   startX, startY,
                                   (int)drawSizeHor, (int)drawSizeVer,
                                   0, flags);

            if (info.Format == Format.Rgb565)
            {
                rgbBitmapTo565(tmp, sourceStride, addr, info);
                Marshal.FreeHGlobal(tmp);
            }

            bitmap.UnlockPixels();
        }

        unsafe static void rgbBitmapTo565(IntPtr source, int sourceStride, IntPtr dest, AndroidBitmapInfo info)
        {
            rgb* srcLine;
            UInt16* dstLine;
            int y, x;
            for (y = 0; y < info.Height; y++)
            {
                srcLine = (rgb*)source;
                dstLine = (UInt16*)dest;
                for (x = 0; x < info.Width; x++)
                {
                    dstLine[x] = rgbTo565(&srcLine[x]);
                }
                source = (IntPtr)((char*)source + sourceStride);
                dest = (IntPtr)((char*)dest + info.Stride);
            }
        }

        unsafe static UInt16 rgbTo565(rgb* color)
        {
            return (ushort)(((color->red >> 3) << 11) | ((color->green >> 2) << 5) | (color->blue >> 3));
        }

        public static void RenderPage(this PdfPage page, Surface surface, float systemDpi,
                           int startX, int startY, int drawSizeX, int drawSizeY,
                           bool renderAnnot)
        {
            lock (o)
            {
                try
                {
                    nativeRenderPage(page.Page, surface, systemDpi,
                            startX, startY, drawSizeX, drawSizeY, renderAnnot);
                }
                catch (NullPointerException e)
                {
                    Log.Error(nameof(PdfPageExtension), "mContext may be null");
                    e.PrintStackTrace();
                }
                catch (Java.Lang.Exception e)
                {
                    Log.Error(nameof(PdfPageExtension), "Exception throw from native");
                    e.PrintStackTrace();
                }
            }
        }

        private static void nativeRenderPage(FpdfPageT page, Surface surface, float systemDpi, int startX, int startY, int drawSizeX, int drawSizeY, bool renderAnnot)
        {
            var nativeWindow = AndroidNative.ANativeWindow_fromSurface(JNIEnv.Handle, surface.Handle);
            if (nativeWindow == default)
            {
                Log.Error(nameof(PdfPageExtension), "native window pointer null");
                return;
            }

            if (AndroidNative.ANativeWindow_getFormat(nativeWindow) != AndroidNative.WINDOW_FORMAT_RGBA_8888)
            {
                Log.Error(nameof(PdfPageExtension), "Set format to RGBA_8888");
                AndroidNative.ANativeWindow_setBuffersGeometry(nativeWindow,
                                                  AndroidNative.ANativeWindow_getWidth(nativeWindow),
                                                  AndroidNative.ANativeWindow_getHeight(nativeWindow),
                                                  AndroidNative.WINDOW_FORMAT_RGBA_8888);
            }

            AndroidNative.ANativeWindow_Buffer buffer;
            int ret;
            if ((ret = AndroidNative.ANativeWindow_lock(nativeWindow, out buffer, IntPtr.Zero)) != 0)
            {
                //LOGE("Locking native window failed: %s", strerror(ret * -1));
                Log.Error(nameof(PdfPageExtension), "Locking native window failed: %s");
                return;
            }

            renderPageInternal(page, buffer,
                       (int)startX, (int)startY,
                       buffer.width, buffer.height,
                       (int)drawSizeX, (int)drawSizeY,
                       (bool)renderAnnot);

            AndroidNative.ANativeWindow_unlockAndPost(nativeWindow);
            AndroidNative.ANativeWindow_release(nativeWindow);
        }

        private static void renderPageInternal(FpdfPageT page, AndroidNative.ANativeWindow_Buffer windowBuffer, int startX, int startY, int canvasHorSize, int canvasVerSize, int drawSizeHor, int drawSizeVer, bool renderAnnot)
        {
            var pdfBitmap = PDFiumCore.fpdfview.FPDFBitmapCreateEx(canvasHorSize, canvasVerSize,
                                                 (int)FPDFBitmapFormat.BGRA,
                                                 windowBuffer.bits, (int)(windowBuffer.stride) * 4);

            /*LOGD("Start X: %d", startX);
            LOGD("Start Y: %d", startY);
            LOGD("Canvas Hor: %d", canvasHorSize);
            LOGD("Canvas Ver: %d", canvasVerSize);
            LOGD("Draw Hor: %d", drawSizeHor);
            LOGD("Draw Ver: %d", drawSizeVer);*/

            if (drawSizeHor < canvasHorSize || drawSizeVer < canvasVerSize)
            {
                fpdfview.FPDFBitmapFillRect(pdfBitmap, 0, 0, canvasHorSize, canvasVerSize,
                                     0x848484FF); //Gray
            }

            int baseHorSize = (canvasHorSize < drawSizeHor) ? canvasHorSize : drawSizeHor;
            int baseVerSize = (canvasVerSize < drawSizeVer) ? canvasVerSize : drawSizeVer;
            int baseX = (startX < 0) ? 0 : startX;
            int baseY = (startY < 0) ? 0 : startY;
            int flags = (int)RenderFlags.FPDF_REVERSE_BYTE_ORDER;

            if (renderAnnot)
            {
                flags |= (int)RenderFlags.RenderAnnotations;
            }

            fpdfview.FPDFBitmapFillRect(pdfBitmap, baseX, baseY, baseHorSize, baseVerSize,
                                 0xFFFFFFFF); //White

            fpdfview.FPDF_RenderPageBitmap(pdfBitmap, page,
                                   startX, startY,
                                   drawSizeHor, drawSizeVer,
                                   0, flags);
        }

        /// <summary>
        /// https://juejin.cn/post/6844903442939412493#comment
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap DecodeStream(Stream ins, int width, int height)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeStream(ins, null, options);
            float srcWidth = options.OutWidth;
            float srcHeight = options.OutHeight;
            int inSampleSize = 1;

            if (srcHeight > height || srcWidth > width)
            {
                if (srcWidth > srcHeight)
                {
                    inSampleSize = (int)Math.Round(srcHeight / height);
                }
                else
                {
                    inSampleSize = (int)Math.Round(srcWidth / width);
                }
            }

            options.InJustDecodeBounds = false;
            options.InSampleSize = inSampleSize;

            return BitmapFactory.DecodeStream(ins, null, options);
        }

        /// <summary>
        /// https://www.jianshu.com/p/67cfd38c52dc
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static async Task<Bitmap> DownsizeLoad(Stream stream, int reqWidth, int reqHeight)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            await BitmapFactory.DecodeStreamAsync(stream, null, options);
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
            options.InJustDecodeBounds = false;
            return await BitmapFactory.DecodeStreamAsync(stream, null, options);
        }

        /// <summary>
        /// 得到图片在宽和高，再根据请求的目标显示区域的大小计算出缩放比例
        /// https://www.jianshu.com/p/67cfd38c52dc
        /// </summary>
        /// <param name="options"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;
            if (height > reqHeight || width > reqWidth)
            {
                // Calculate ratios of height and width to requested height and width,计算出实际宽高和目标宽高的比率
                int heightRatio = (int)Math.Round((float)height / (float)reqHeight);
                // 选择宽和高中最小的比率作为inSampleSize的值，这样可以保证最终图片的宽和高一定都会大于等于目标的宽和高。
                int widthRatio = (int)Math.Round((float)width / (float)reqWidth);
                inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
                // Anything more than 2x the requested pixels we'll sample down
                float totalPixels = width * height;
                // further
                float totalReqPixelsCap = reqWidth * reqHeight * 2;
                while (totalPixels / (inSampleSize * inSampleSize) > totalReqPixelsCap)
                {
                    inSampleSize++;
                }
            }
            return inSampleSize;
        }

        //参考https://juejin.cn/post/6844903442939412493#comment
        public static void WriteBitmapToFile(string filePath, Bitmap b, int quality)
        {
            try
            {
                var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                b.Compress(Bitmap.CompressFormat.Jpeg, quality, fs);
                fs.Flush();
                fs.Close();
            }
            catch (Java.IO.IOException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}
#endif