using PDFiumCore;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// A free text annotation (PDF 1.3) displays text directly on the page. Unlike an ordinary text annotation (see PdfTextAnnotation), a free text annotation has no open or closed state; instead of being displayed in a pop-up window, the text is always visible.
    /// </summary>
    public class PdfTextAnnotation : PdfAnnotation, IColorAnnotation
    {
        private const string TAG = nameof(PdfTextAnnotation);
        public PdfTextAnnotation()
            : base(PdfAnnotationSubtype.Text)
        {
            // Set some default
            // TextFont = "Arial";
            // TextSize = 12;
        }

        /// <summary>
        /// FreeText的设置有两种方式，一种设置StringValue，一种通过文本对象，后者可以控制更多属性
        /// </summary>
        /// <param name="page"></param>
        /// <param name="annotation"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal PdfTextAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            Text = GetStringValue();
            //颜色
            AnnotColor = GetAnnotColor();
        }

        public string? Text { get; set; }

        public Color? AnnotColor { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            //基类创建了native注释对象
            base.AddToPage(page);
            SetStringValue(Text);
            SetAnnotColor(AnnotColor);
        }
    }
}
