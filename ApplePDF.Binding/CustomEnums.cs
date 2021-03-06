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
    public enum FPDFAnnotationSubtype
    {
        UNKNOWN = 0,
        TEXT = 1,
        LINK = 2,
        FREETEXT = 3,
        LINE = 4,
        SQUARE = 5,
        CIRCLE = 6,
        //多边形
        POLYGON = 7,
        //折线
        PDPOLYLINE = 8,
        HIGHLIGHT = 9,
        UNDERLINE = 10,
        SQUIGGLY = 11,
        STRIKEOUT = 12,
        STAMP = 13,
        CARET = 14,
        INK = 15,
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
        Incremental = 1,
        NoIncremental = 2,
        RemoveSecurity = 3,
    }


    #region fpdf_edit
    /// <summary>
    /// The page object constants.
    /// </summary>
    public enum PdfPageObjectTypeFlag
    {
        UNKNOWN = 0,
        TEXT = 1,
        PATH = 2,
        IMAGE = 3,
        SHADING = 4,
        FORM = 5,
    }

    /// <summary>
    ///  color space families.
    /// </summary>
    public enum PdfColorSpaceFlag
    {

        UNKNOWN = 0,
        DEVICEGRAY = 1,
        DEVICERGB = 2,
        DEVICECMYK = 3,
        CALGRAY = 4,
        CALRGB = 5,
        LAB = 6,
        ICCBASED = 7,
        SEPARATION = 8,
        DEVICEN = 9,
        INDEXED = 10,
        PATTERN = 11
    }

    // The path segment constants.
    public enum PdfSegmentFlag
    {
        FPDF_SEGMENT_UNKNOWN = -1,
        FPDF_SEGMENT_LINETO = 0,
        FPDF_SEGMENT_BEZIERTO = 1,
        FPDF_SEGMENT_MOVETO = 2
    }
    public enum PdfFillMode
    {
        FPDF_FILLMODE_NONE = 0,
        FPDF_FILLMODE_ALTERNATE = 1,
        FPDF_FILLMODE_WINDING = 2
    }

    public enum PdfFont
    {
        FPDF_FONT_TYPE1 = 1,
        FPDF_FONT_TRUETYPE = 2
    }

    public enum LineCap
    {
        FPDF_LINECAP_BUTT = 0,
        FPDF_LINECAP_ROUND = 1,
        FPDF_LINECAP_PROJECTING_SQUARE = 2
    }

    public enum PdfLineJoin
    {
        FPDF_LINEJOIN_MITER = 0,
        FPDF_LINEJOIN_ROUND = 1,
        FPDF_LINEJOIN_BEVEL = 2
    }

    /// <summary>
    /// See FPDF_SetPrintMode() for descriptions.
    /// </summary>
    public enum PdfPrintMode
    {
        FPDF_PRINTMODE_EMF = 0,
        FPDF_PRINTMODE_TEXTONLY = 1,
        FPDF_PRINTMODE_POSTSCRIPT2 = 2,
        FPDF_PRINTMODE_POSTSCRIPT3 = 3,
        FPDF_PRINTMODE_POSTSCRIPT2_PASSTHROUGH = 4,
        FPDF_PRINTMODE_POSTSCRIPT3_PASSTHROUGH = 5,
        FPDF_PRINTMODE_EMF_IMAGE_MASKS = 6,
        FPDF_PRINTMODE_POSTSCRIPT3_TYPE42 = 7,
        FPDF_PRINTMODE_POSTSCRIPT3_TYPE42_PASSTHROUGH = 8
    }
    #endregion
}
