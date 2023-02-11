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
        public class Constant
        {
            public class CommonKey
            {
                internal const string kPopupKey = "Popup";
            }
        }

        internal PdfAnnotation Parent;

        public PdfPopupAnnotation(PdfPage page, PdfAnnotation parentAnnotation, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            Parent = parentAnnotation;
        }

        public string Text 
        {
            get
            {
                return Parent.GetStringValueFromKey(PdfAnnotation.Constant.CommonKey.kContents);
            }

            set
            {
                Parent.SetStringValueForKey(value, PdfAnnotation.Constant.CommonKey.kContents);
            }
        }
    }
}
