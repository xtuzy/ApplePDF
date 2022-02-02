using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pdf.Net.PdfKit.Annotation
{
    public class PdfFreeTextAnnotation : PdfAnnotation
    {
        public PdfFreeTextAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfFreeTextAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index) : base(page, annotation, type, index)
        {
            bool success = false;
           
            // Get Text
            var buffer = new ushort[100];
            var result = fpdf_annot.FPDFAnnotGetStringValue(Annotation, ConstDictionaryKeyContents, ref buffer[0], (uint)buffer.Length);
            if (result == 0)
            {
                throw new NotImplementedException();
            }
            else if (result == 2)
            {
                throw new NotImplementedException();
            }
            else
            {
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        Text = new string((char*)dataPtr, 0, (int)result);
                }
            }
            //Get TextSize

        }

        public string Text { get; set; }

        public float TextSize { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            //Set Text
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(Text);
            ushort[] value = new ushort[Text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);
            fpdf_annot.FPDFAnnotSetStringValue(Annotation, ConstDictionaryKeyContents, ref value[0]);
        }
    }
}
