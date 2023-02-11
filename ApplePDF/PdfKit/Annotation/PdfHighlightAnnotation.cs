using PDFiumCore;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfHighlightAnnotation : PdfMarkupAnnotation, IPdfHighlightAnnotation
    {
        /// <summary>
        /// 获取附着在其上的Popup注释
        /// </summary>
        public PdfPopupAnnotation PopupAnnotation
        {
            get
            {
                var havePopup = fpdf_annot.FPDFAnnotHasKey(Annotation, PdfPopupAnnotation.Constant.CommonKey.kPopupKey);
                if (havePopup == 1)
                {
                    return new PdfPopupAnnotation(this.Page, this, null, PdfAnnotationSubtype.Popup, -1);
                }
                else
                {
                    if (GetStringValueFromKey() != string.Empty)
                        return new PdfPopupAnnotation(this.Page, this, null, PdfAnnotationSubtype.Popup, -1);
                    return null;
                }
            }
        }

        internal PdfHighlightAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
        }

        /// <summary>
        /// Default color is yellow. Edge注释的黄色是#fff066
        /// </summary>
        public Color? HighlightColor { get => GetAnnotColor(); set => SetAnnotColor(value); }

        /// <summary>
        /// 当前Pdfium未提供连接Popup和Marktype的方式, 此创建不会连接彼此, Popup是空内容, 请给Popup设置text(实际设置了Parent内容), 来假定拥有Popup
        /// </summary>
        /// <returns></returns>
        public PdfPopupAnnotation AddPopupAnnotation()
        {
            var annotationType = PdfAnnotationSubtype.Popup;
            // 创建注释到Pdfium
            var annotation = fpdf_annot.FPDFPageCreateAnnot(Page.Page, (int)annotationType);
            if (annotation == null)
            {
                Debug.WriteLine($"Cant't create new {annotationType} annotation");
                return null;
            }
            var index = fpdf_annot.FPDFPageGetAnnotIndex(Page.Page, annotation);
            if (index == -1)
            {
                Debug.WriteLine($"Cant't create new {annotationType} annotation");
                return null;
            }
            return new PdfPopupAnnotation(this.Page, this, annotation, annotationType, index);
        }
    }
}
