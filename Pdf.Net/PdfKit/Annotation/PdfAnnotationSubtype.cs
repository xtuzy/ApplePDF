using PDFiumCore;
namespace Pdf.Net.PdfKit.Annotation
{
    /// <summary>
    /// The type of annotation, such as circle, text, or ink.
    /// </summary>
    public enum PdfAnnotationSubtype
    {
        Text=FPDF_AnnotationSubtype.FPDF_ANNOT_TEXT,
        Link=FPDF_AnnotationSubtype.FPDF_ANNOT_LINK,
        FreeText=FPDF_AnnotationSubtype.FPDF_ANNOT_FREETEXT,
        Line= FPDF_AnnotationSubtype.FPDF_ANNOT_LINE,
        Square=FPDF_AnnotationSubtype.FPDF_ANNOT_SQUARE,
        Circle=FPDF_AnnotationSubtype.FPDF_ANNOT_CIRCLE,
        Highlight=FPDF_AnnotationSubtype.FPDF_ANNOT_HIGHLIGHT,
        Underline=FPDF_AnnotationSubtype.FPDF_ANNOT_UNDERLINE,
        StrikeOut=FPDF_AnnotationSubtype.FPDF_ANNOT_STRIKEOUT,
        Ink=FPDF_AnnotationSubtype.FPDF_ANNOT_INK,
        Stamp=FPDF_AnnotationSubtype.FPDF_ANNOT_STAMP,
        Popup=FPDF_AnnotationSubtype.FPDF_ANNOT_POPUP,
        Widget=FPDF_AnnotationSubtype.FPDF_ANNOT_WIDGET,
    }
}
