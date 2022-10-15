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
            // 先尝试使用StringValue获取文本，当返回2时就是没有
            ushort[] buffer = new ushort[1];
            var resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, PdfAnnotation.KeyConstant.Common.kContents, ref buffer[0], (uint)0);
            if (resultBytesLength == 0)
            {
                throw new NotImplementedException($"{TAG}:Create PdfFreeTextAnnotation fail, no reason return.");
            }
            else if (resultBytesLength == 2)
            {
                Debug.WriteLine($"{TAG}:By FPDFAnnotGetStringValue() create PdfFreeTextAnnotation fail, because don't find text content in this annotation, next will try use textObject.");
            }
            else
            {
                buffer = new ushort[resultBytesLength];
                resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, PdfAnnotation.KeyConstant.Common.kContents, ref buffer[0], (uint)buffer.Length);
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        Text = new string((char*)dataPtr, 0, (int)(resultBytesLength - 2) / 2);//返回的result长度会有结束符,貌似占用两个byte长度,所以减去,ushort占用两个byte,所以除以2
                }
            }

            //颜色
            AnnotColor = GetAnnotColor();
        }

        public string? Text { get; set; }

        public Color? AnnotColor { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            //基类创建了native注释对象
            base.AddToPage(page);
            //Set Text
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(Text);
            ushort[] value = new ushort[Text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);

            //设置注释本身的StringValue
            var success = fpdf_annot.FPDFAnnotSetStringValue(Annotation, PdfAnnotation.KeyConstant.Common.kContents, ref value[0]) == 1;
            if (!success)
                throw new InvalidOperationException($"{TAG}:Set free text fail");

            SetAnnotColor(AnnotColor);
        }
    }
}
