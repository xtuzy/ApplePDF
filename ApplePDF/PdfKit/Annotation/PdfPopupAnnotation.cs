using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// 参考:https://pdfium.patagames.com/help/html/T_Patagames_Pdf_Net_Annotations_PdfPopupAnnotation.htm
    /// PopupAnnotation附着在其它注释上,用于编辑父级的文本,它本身没有外观.Pdf软件中弹出的编辑框是软件自定义的输入框.
    /// </summary>
    public class PdfPopupAnnotation : PdfAnnotation
    {
        internal const string kPopupKey = "Popup";

        internal PdfAnnotation Parent;

        public string Text;

        public PdfPopupAnnotation(PdfAnnotation parent) : base(PdfAnnotationSubtype.Popup)
        {
            parent = Parent;
        }

        internal override void AddToPage(PdfPage page)
        {
           base.AddToPage(page);
        }
    }
}
