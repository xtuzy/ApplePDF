using PDFiumCore;
using System;

namespace ApplePDF.PdfKit
{
    public class PdfBitmap : IDisposable
    {
        public PdfBitmap(int w, int h, bool alpha)
        {
            Bitmap = fpdfview.FPDFBitmapCreate(w, h, alpha == true ? 1 : 0);     
        }

        internal PdfBitmap(FpdfBitmapT bitmapT)
        {
            Bitmap = bitmapT;
        }

        public FpdfBitmapT Bitmap { get; private set; }

        public int Stride
        {
            get
            {
                return Bitmap != null ? fpdfview.FPDFBitmapGetStride(Bitmap) : 0;
            }
        }
        
        public int Height
        {
            get
            {
                return Bitmap != null ? fpdfview.FPDFBitmapGetHeight(Bitmap) : 0;
            }
        }
        
        public int Width
        {
            get
            {
                return Bitmap != null ? fpdfview.FPDFBitmapGetWidth(Bitmap) : 0;
            }
        }

        public IntPtr Buffer
        {
            get
            {
                return fpdfview.FPDFBitmapGetBuffer(Bitmap);
            }
        }

        public int BufferLength
        {
            get
            {
                return Stride * Height;
            }
        }

        public FPDFBitmapFormat Format
        {
            get
            {
                return (FPDFBitmapFormat)fpdfview.FPDFBitmapGetFormat(Bitmap);
            }
        }

        public void Dispose()
        {
            fpdfview.FPDFBitmapDestroy(Bitmap);
            Bitmap = null;
        }
    }
}
