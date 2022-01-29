using System;

namespace PDFiumCore
{
    /// <summary>
    /// Page rendering flags. They can be combined with bit-wise OR.
    /// </summary>
    [Flags]
    public enum RenderFlags
    {
        /*        // Page rendering flags. They can be combined with bit-wise OR.
                //
                // Set if annotations are to be rendered.
        #define FPDF_ANNOT 0x01
                // Set if using text rendering optimized for LCD display. This flag will only
                // take effect if anti-aliasing is enabled for text.
        #define FPDF_LCD_TEXT 0x02
                // Don't use the native text output available on some platforms
        #define FPDF_NO_NATIVETEXT 0x04
                // Grayscale output.
        #define FPDF_GRAYSCALE 0x08
                // Obsolete, has no effect, retained for compatibility.
        #define FPDF_DEBUG_INFO 0x80
                // Obsolete, has no effect, retained for compatibility.
        #define FPDF_NO_CATCH 0x100
                // Limit image cache size.
        #define FPDF_RENDER_LIMITEDIMAGECACHE 0x200
                // Always use halftone for image stretching.
        #define FPDF_RENDER_FORCEHALFTONE 0x400
                // Render for printing.
        #define FPDF_PRINTING 0x800
                // Set to disable anti-aliasing on text. This flag will also disable LCD
                // optimization for text rendering.
        #define FPDF_RENDER_NO_SMOOTHTEXT 0x1000
                // Set to disable anti-aliasing on images.
        #define FPDF_RENDER_NO_SMOOTHIMAGE 0x2000
                // Set to disable anti-aliasing on paths.
        #define FPDF_RENDER_NO_SMOOTHPATH 0x4000
                // Set whether to render in a reverse Byte order, this flag is only used when
                // rendering to a bitmap.
        #define FPDF_REVERSE_BYTE_ORDER 0x10
                // Set whether fill paths need to be stroked. This flag is only used when
                // FPDF_COLORSCHEME is passed in, since with a single fill color for paths the
                // boundaries of adjacent fill paths are less visible.
        #define FPDF_CONVERT_FILL_TO_STROKE 0x20*/

        /// <summary>
        /// No flag.
        /// see https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfRenderFlags.cs
        /// </summary>
        None = 0,
        /// <summary>
        /// FPDF_ANNOT: Set if annotations are to be rendered.
        /// </summary>
        RenderAnnotations = 0x01,

        /// <summary>
        /// FPDF_LCD_TEXT: Set if using text rendering optimized for LCD display. This flag will only take effect if anti-aliasing is enabled for text.
        /// </summary>
        OptimizeTextForLcd = 0x02,

        /// <summary>
        /// FPDF_NO_NATIVETEXT: Don't use the native text output available on some platforms
        /// </summary>
        NoNativeText = 0x04,

        /// <summary>
        /// FPDF_GRAYSCALE: Grayscale output.(»ÒÉ«)
        /// </summary>
        Grayscale = 0x08,

        /// <summary>
        /// // FPDF_RENDER_LIMITEDIMAGECACHE: Limit image cache size
        /// </summary>
        LimitImageCacheSize = 0x200,

        /// <summary>
        /// FPDF_RENDER_FORCEHALFTONE: Always use halftone for image stretching
        /// </summary>
        ForceHalftone = 0x400,

        /// <summary>
        /// FPDF_PRINTING: Render for printing
        /// </summary>
        RenderForPrinting = 0x800,

        /// <summary>
        /// FPDF_RENDER_NO_SMOOTHTEXT: Set to disable anti-aliasing on text. This flag will also disable LCD optimization for text rendering
        /// </summary>
        DisableTextAntialiasing = 0x1000,


        /// <summary>
        /// FPDF_RENDER_NO_SMOOTHIMAGE: Set to disable anti-aliasing on images.
        /// </summary>
        DisableImageAntialiasing = 0x2000,

        /// <summary>
        /// FPDF_RENDER_NO_SMOOTHPATH: Set to disable anti-aliasing on paths.
        /// </summary>
        DisablePathAntialiasing = 0x4000
    }
}