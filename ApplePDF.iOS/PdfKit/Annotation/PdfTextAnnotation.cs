using ApplePDF.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// A free text annotation (PDF 1.3) displays text directly on the page. Unlike an ordinary text annotation (see PdfTextAnnotation), a free text annotation has no open or closed state; instead of being displayed in a pop-up window, the text is always visible.
    /// </summary>
    public class PdfTextAnnotation : PdfAnnotation
    {
        private const string TAG = nameof(PdfTextAnnotation);

        /// <summary>
        /// FreeText的设置有两种方式，一种设置StringValue，一种通过文本对象，后者可以控制更多属性
        /// </summary>
        /// <param name="page"></param>
        /// <param name="annotation"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal PdfTextAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type) : base(page, annotation, type)
        {
        }

        public string? Text { get => Annotation.Contents; set => Annotation.Contents = value; }

        public Color? TextColor { get=> Annotation.FontColor.ToColor() ; set=> Annotation.Color = value.Value.ToUIColor(); }
    }
}
