using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// A free text annotation (PDF 1.3) displays text directly on the page. Unlike an ordinary text annotation (see PdfTextAnnotation), a free text annotation has no open or closed state; instead of being displayed in a pop-up window, the text is always visible.
    /// </summary>
    public class PdfFreeTextAnnotation : PdfAnnotation_ReadonlyPdfPageObj
    {
        private const string TAG = nameof(PdfFreeTextAnnotation);
        public PdfFreeTextAnnotation()
            : base(PdfAnnotationSubtype.FreeText)
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
        internal PdfFreeTextAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            // 先尝试使用StringValue获取文本，当返回2时就是没有
            var buffer = new ushort[100];
            var result = fpdf_annot.FPDFAnnotGetStringValue(Annotation, ConstDictionaryKeyContents, ref buffer[0], (uint)buffer.Length);
            if (result == 0)
            {
                throw new NotImplementedException($"{TAG}:Create PdfFreeTextAnnotation fail, no reason return.");
            }
            else if (result == 2)
            {
                Debug.WriteLine($"{TAG}:By FPDFAnnotGetStringValue() create PdfFreeTextAnnotation fail, because don't find text content in this annotation, next will try use textObject.");
            }
            else
            {
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        Text = new string((char*)dataPtr, 0, (int)result);
                }
            }

            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var pdfPageObjs = new PdfPageObj[objectCount];
                PdfPageObjs = pdfPageObjs;
                for (int objIndex = 0; objIndex < objectCount; objIndex++)
                {
                    var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                    if (obj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                        if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                        {
                            pdfPageObjs[objIndex] = new PdfPageTextObj(obj);
                        }
                    }
                }
            }
        }

        public string? Text { get; set; }

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
            var success = fpdf_annot.FPDFAnnotSetStringValue(Annotation, ConstDictionaryKeyContents, ref value[0]) == 1;
            if (!success)
                throw new InvalidOperationException($"{TAG}:Set free text fail");
        }
    }
}
