using ApplePDF.Extensions;
using Foundation;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfHighlightAnnotation : PdfMarkupAnnotation, IPdfHighlightAnnotation
    {
        public PdfPopupAnnotation PopupAnnotation
        {
            get
            {
                var popup = Annotation.Popup;
                if (popup == null)
                {
                    if(Annotation.Contents != null)
                    {
                        //如果Hightlight有文字内容，那么也当作popup，这是因为测试是发现iOS未识别popup
                        return new PdfPopupAnnotation(Page, this, popup, PdfAnnotationSubtype.Popup);
                    }
                    return null;
                }
                else
                {
                    return new PdfPopupAnnotation(Page, this, popup, PdfAnnotationSubtype.Popup);
                }
            }
        }

        internal PdfHighlightAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {

        }

        /// <summary>
        /// Default color is yellow. Edge注释的黄色是#fff066
        /// </summary>
        public Color? HighlightColor { get => Annotation.BackgroundColor.ToColor(); set => Annotation.BackgroundColor = value.Value.ToUIColor(); }

        public PdfPopupAnnotation AddPopupAnnotation()
        {
#if MACOS
            var annotation = new iOSPdfKit.PdfAnnotationPopup();
#else
            var annotation = new PlatformPdfAnnotation();
#endif
            Annotation.Popup = annotation;
            return new PdfPopupAnnotation(this.Page, this, annotation, PdfAnnotationSubtype.Popup);
        }
    }
}
