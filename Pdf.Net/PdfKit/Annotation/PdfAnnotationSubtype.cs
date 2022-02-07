using PDFiumCore;
namespace Pdf.Net.PdfKit.Annotation
{
    /// <summary>
    /// The type of annotation, such as circle, text, or ink.
    /// </summary>
    public enum PdfAnnotationSubtype
    {
        Text= PDFiumCore.FPDFAnnotationSubtype.TEXT,
        Link= PDFiumCore.FPDFAnnotationSubtype.LINK,
        FreeText= PDFiumCore.FPDFAnnotationSubtype.FREETEXT,
        Line= PDFiumCore.FPDFAnnotationSubtype.LINE,
        /// <summary>
        /// 方形
        /// </summary>
        Square= PDFiumCore.FPDFAnnotationSubtype.SQUARE,
        Circle= PDFiumCore.FPDFAnnotationSubtype.CIRCLE,
        Highlight= PDFiumCore.FPDFAnnotationSubtype.HIGHLIGHT,
        Underline= PDFiumCore.FPDFAnnotationSubtype.UNDERLINE,
        StrikeOut= PDFiumCore.FPDFAnnotationSubtype.STRIKEOUT,
        Ink= PDFiumCore.FPDFAnnotationSubtype.INK,
        /// <summary>
        /// 图章
        /// </summary>
        Stamp= PDFiumCore.FPDFAnnotationSubtype.STAMP,
        Popup= PDFiumCore.FPDFAnnotationSubtype.FPDF_ANNOT_POPUP,
        Widget= PDFiumCore.FPDFAnnotationSubtype.FPDF_ANNOT_WIDGET,
        /// <summary>
        /// 多边形
        /// </summary>
        Polygon = PDFiumCore.FPDFAnnotationSubtype.POLYGON,
        /// <summary>
        /// 折线
        /// </summary>
        Polyline = PDFiumCore.FPDFAnnotationSubtype.PDPOLYLINE
    }
}
