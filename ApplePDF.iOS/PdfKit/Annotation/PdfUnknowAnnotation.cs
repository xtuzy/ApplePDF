using Foundation;
using System.Collections.Generic;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfUnknowAnnotation : PdfAnnotation
    {
        public List<PdfRectangleF> UnderlineLocation = new List<PdfRectangleF>();

        internal PdfUnknowAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }

        public string CustomAnnotationType
        {
            get => Annotation.GetValue<NSString>(iOSPdfKit.PdfAnnotationKey.Subtype).ToString();
            set => Annotation.SetValue<NSString>(new NSString(value), iOSPdfKit.PdfAnnotationKey.Subtype);
        }
    }
}
