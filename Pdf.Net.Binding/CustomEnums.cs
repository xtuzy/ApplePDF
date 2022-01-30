using System;
using System.Collections.Generic;
using System.Text;

namespace PDFiumCore
{
    /// <summary>
    /// More DIB formats,Use at FPDFBitmap_CreateEx.
    /// </summary>
    public enum FPDFBitmapFormat
    {
        /// <summary>
        /// Unknown or unsupported format.
        /// From FPDFBitmap_Unknown 0.
        /// </summary>
        Unknown,

        /// <summary>
        /// Gray scale bitmap, one byte per pixel.
        /// From FPDFBitmap_Gray 1.
        /// </summary>
        Gray,

        /// <summary>
        /// 3 bytes per pixel, byte order: blue, green, red.
        /// From FPDFBitmap_BGR 2.
        /// </summary>
        BGR,

        /// <summary>
        /// 4 bytes per pixel, byte order: blue, green, red, unused.
        /// From FPDFBitmap_BGRx 3.
        /// </summary>
        BGRx,

        /// <summary>
        /// 4 bytes per pixel, byte order: blue, green, red, alpha.
        /// From FPDFBitmap_BGRA 4.
        /// </summary>
        BGRA,
    }

    /// <summary>
    /// https://github.com/chromium/pdfium/blob/3e36f68831431bf497babc74075cd69af5fd9823/public/fpdf_annot.h#L24
    /// </summary>
    public  enum  FPDF_AnnotationSubtype
    {
        FPDF_ANNOT_UNKNOWN = 0,
        FPDF_ANNOT_TEXT = 1,
        FPDF_ANNOT_LINK = 2,
        FPDF_ANNOT_FREETEXT = 3,
        FPDF_ANNOT_LINE = 4,
        FPDF_ANNOT_SQUARE = 5,
        FPDF_ANNOT_CIRCLE = 6,
        FPDF_ANNOT_POLYGON = 7,
        FPDF_ANNOT_POLYLINE = 8,
        FPDF_ANNOT_HIGHLIGHT = 9,
        FPDF_ANNOT_UNDERLINE = 10,
        FPDF_ANNOT_SQUIGGLY = 11,
        FPDF_ANNOT_STRIKEOUT = 12,
        FPDF_ANNOT_STAMP = 13,
        FPDF_ANNOT_CARET = 14,
        FPDF_ANNOT_INK = 15,
        FPDF_ANNOT_POPUP = 16,
        FPDF_ANNOT_FILEATTACHMENT = 17,
        FPDF_ANNOT_SOUND = 18,
        FPDF_ANNOT_MOVIE = 19,
        FPDF_ANNOT_WIDGET = 20,
        FPDF_ANNOT_SCREEN = 21,
        FPDF_ANNOT_PRINTERMARK = 22,
        FPDF_ANNOT_TRAPNET = 23,
        FPDF_ANNOT_WATERMARK = 24,
        FPDF_ANNOT_THREED = 25,
        FPDF_ANNOT_RICHMEDIA = 26,
        FPDF_ANNOT_XFAWIDGET = 27,
    }

    public enum PdfRotate
    {
        Degree0 = 0,
        Degree90 = 1,
        Degree180 = 2,
        Degree270 = 3,
    }

    /// <summary>
    /// Pdf save flag
    /// </summary>
    public enum PdfSaveFlag
    {
        Incremental=1,
        NoIncremental=2,
        RemoveSecurity=3,
    }
}
