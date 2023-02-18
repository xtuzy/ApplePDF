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

    #region fpdf_annot
    /// <summary>
    /// https://github.com/chromium/pdfium/blob/main/public/fpdf_annot.h
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

    /// <summary>
    /// <see cref="https://pdfium.patagames.com/help/html/T_Patagames_Pdf_Enums_AnnotationFlags.htm"/> and <see cref="https://github.com/chromium/pdfium/blob/main/public/fpdf_annot.h"/>
    /// </summary>
    public enum FPDFAnnotationFlag
    {
        /// <summary>
        /// No any flags are setted.
        /// </summary>
        None = 0,
        /// <summary>
        /// If set, do not display the annotation if it does not belong to one of the standard annotation types and no annotation handler is available. If clear, display such an unknown annotation using an appearance stream specified by its appearance dictionary, if any.
        /// </summary>
        Invisible = 1,
        /// <summary>
        /// If set, do not display or print the annotation or allow it to interact with the user, regardless of its annotation type or whether an annotation handler is available.In cases where screen space is limited, the ability to hide and show annotations selectively can be used in combination with appearance streams to display auxiliary pop-up information similar in function to online help systems.
        /// </summary>
        Hidden = 2,
        /// <summary>
        /// If set, print the annotation when the page is printed.If clear, never print the annotation, regardless of whether it is displayed on the screen.This can be useful, for example, for annotations representing interactive pushbuttons, which would serve no meaningful purpose on the printed page.
        /// </summary>
        Print = 4,
        /// <summary>
        /// If set, do not scale the annotation’s appearance to match the magnification of the page.The location of the annotation on the page (defined by the upper-left corner of its annotation rectangle) remains fixed, regardless of the page magnification.See remarks for further discussion.
        /// </summary>
        NoZoom = 8,
        /// <summary>
        /// If set, do not rotate the annotation’s appearance to match the rotation of the page.The upper - left corner of the annotation rectangle remains in a fixed location on the page, regardless of the page rotation.See remarks for further discussion.
        /// </summary>
        NoRotate = 16,
        /// <summary>
        /// If set, do not display the annotation on the screen or allow it to interact with the user.The annotation may be printed(depending on the setting of the Print flag) but should be considered hidden for purposes of onscreen display and user interaction.
        /// </summary>
        NoView = 32,
        /// <summary>
        /// If set, do not allow the annotation to interact with the user.The annotation may be displayed or printed(depending on the settings of the NoView and Print flags) but should not respond to mouse clicks or change its appearance in response to mouse motions.
        /// Note:This flag is ignored for widget annotations; its function is subsumed by the ReadOnly flag of the associated form field
        /// </summary>
        ReadOnly = 64,
        /// <summary>
        /// If set, do not allow the annotation to be deleted or its properties(including position and size) to be modified by the user.However, this flag does not restrict changes to the annotation’s contents, such as the value of a form field.
        /// </summary>
        Locked = 128,
        /// <summary>
        /// If set, invert the interpretation of the NoView flag for certain events. A typical use is to have an annotation that appears only when a mouse cursor is held over it.
        /// </summary>
        ToggleNoView = 256,
        /// <summary>
        /// If set, do not allow the contents of the annotation to be modified by the user.This flag does not restrict deletion of the annotation or changes to other annotation properties, such as position and size.
        /// </summary>
        LockedContents = 512,
    }
    #endregion

    public enum PdfRotate
    {
        Degree0 = 0,
        Degree90 = 1,
        Degree180 = 2,
        Degree270 = 3,
    }

    /// <summary>
    /// Pdf save flag, for <see cref="fpdfsave.FPDF_SaveAsCopy(System.IntPtr, fpdfsave.FpdfStreamWriter, uint)"/>
    /// </summary>
    public enum PdfSaveFlag
    {
        /// <summary>
        /// Notice, this default flag not in Pdfium api, i see Pdfium's test all use 0, so add it.
        /// </summary>
        DefaultInTest = 0,
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

    public enum PdfFontType
    {
        /// <summary>
        /// 参考https://zhuanlan.zhihu.com/p/386035885,Adobe的Type1
        /// </summary>
        FPDF_FONT_TYPE1 = 1,
        /// <summary>
        /// 参考https://zhuanlan.zhihu.com/p/386035885,区别于OpenType,参考https://baike.baidu.com/item/OpenType/10425330,OpenType叫Type2
        /// </summary>
        FPDF_FONT_TRUETYPE = 2
    }

    public enum PdfLineCap
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

    #region fpdf_text
    /// <summary>
    /// <see cref="fpdf_text.FPDFTextGetFontInfo"/>会返回flag，我从PdfReference1.7获取
    /// </summary>
    public enum FontDescriptorFlags
    {
        /// <summary>
        /// All glyphs have the same width (as opposed to proportional or variable-pitch
        /// fonts, which have different widths).
        /// </summary>
        FixedPitch = 1,
        /// <summary>
        /// Glyphs have serifs, which are short strokes drawn at an angle on the top and
        /// bottom of glyph stems. (Sans serif fonts do not have serifs.)
        /// </summary>
        Serif = 2,
        /// <summary>
        /// Font contains glyphs outside the Adobe standard Latin character set. This
        /// flag and the Nonsymbolic flag cannot both be set or both be clear (see be-low).
        /// </summary>
        Symbolic = 3,
        /// <summary>
        /// Glyphs resemble cursive handwriting.
        /// </summary>
        Script = 4,
        /// <summary>
        /// Font uses the Adobe standard Latin character set or a subset of it (see below).
        /// </summary>
        Nonsymbolic = 6,
        /// <summary>
        /// Glyphs have dominant vertical strokes that are slanted.
        /// </summary>
        Italic = 7,
        /// <summary>
        /// Font contains no lowercase letters; typically used for display purposes, such
        /// as for titles or headlines.
        /// </summary>
        AllCap = 17,
        /// <summary>
        /// Font contains both uppercase and lowercase letters. The uppercase letters are
        /// similar to those in the regular version of the same typeface family. The glyphs
        /// for the lowercase letters have the same shapes as the corresponding uppercase
        /// letters, but they are sized and their proportions adjusted so that they have the
        /// same size and stroke weight as lowercase glyphs in the same typeface family.
        /// </summary>
        SmallCap = 18,
        /// <summary>
        /// Font contains both uppercase and lowercase letters. The uppercase letters are
        /// similar to those in the regular version of the same typeface family. The glyphs
        /// for the lowercase letters have the same shapes as the corresponding uppercase
        /// letters, but they are sized and their proportions adjusted so that they have the
        /// same size and stroke weight as lowercase glyphs in the same typeface family.
        /// </summary>
        ForceBold = 19
    }
    #endregion

    #region fpdfview

    /// <summary>
    /// 参考 <see href="https://github.com/chromium/pdfium/blob/main/public/fpdfview.h"/> 
    /// </summary>
    public enum PdfObjectType
    {
        FPDF_OBJECT_UNKNOWN,
        FPDF_OBJECT_BOOLEAN,
        FPDF_OBJECT_NUMBER,
        FPDF_OBJECT_STRING,
        FPDF_OBJECT_NAME,
        FPDF_OBJECT_ARRAY,
        FPDF_OBJECT_DICTIONARY,
        FPDF_OBJECT_STREAM,
        FPDF_OBJECT_NULLOBJ,
        FPDF_OBJECT_REFERENCE
    }
    #endregion
}
