using ApplePDF.PdfKit.Annotation;

namespace ApplePDF.PdfKit
{
    public interface IPdfAnnotation
    {
        PlatformPdfAnnotation Annotation { get; }
        PdfAnnotationSubtype AnnotationType { get; }
        PdfRectangleF AnnotBox { get; set; }
    }
}