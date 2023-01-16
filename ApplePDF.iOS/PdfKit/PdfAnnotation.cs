namespace ApplePDF.PdfKit
{
    public class PdfAnnotation
    {
        internal PdfAnnotation(iOSPdfKit.PdfAnnotation annotation) 
        { 
            Annotation = annotation;
        }

        public iOSPdfKit.PdfAnnotation Annotation { get; private set; }
    }
}