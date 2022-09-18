using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfFreeTextAnnotation : PdfAnnotation
    {
        public PdfFreeTextAnnotation(PdfAnnotationSubtype type) : base(type)
        {
            //Set some default
            TextFont = "Arial";
            TextSize = 12;
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

            // 颜色
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount == 1)
            {
                var freeTextObj = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                if (freeTextObj != null)
                {
                    var objectType = fpdf_edit.FPDFPageObjGetType(freeTextObj);
                    if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                    {
                        success = fpdf_edit.FPDFPageObjGetFillColor(freeTextObj, ref R, ref G, ref B, ref A) == 1;
                        if (success)
                            FillColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        else
                            Debug.WriteLine("No fill color");
                        success = fpdf_edit.FPDFPageObjGetStrokeColor(freeTextObj, ref R, ref G, ref B, ref A) == 1;

                        if (success)
                            StrokeColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        else
                            Debug.WriteLine("No stroke color");
                    }
                }
            }
            else if (objectCount == 0)
                ;//测试mytest_4_freetextannotation.pdf时,为0时貌似也可能正确,这个注释好像是不显示的
            else
            {
                throw new NotImplementedException("Not only one object, don't know how to get correct object");
            }
            //Get TextSize
        }

        public string? Text { get; set; }

        /// <summary>
        /// Default is 12
        /// </summary>
        public float TextSize { get; set; }

        /// <summary>
        /// Default is Arial
        /// </summary>
        public string TextFont { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
            //Set Text
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(Text);
            ushort[] value = new ushort[Text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);
            //fpdf_annot.FPDFAnnotSetStringValue(Annotation, ConstDictionaryKeyContents, ref value[0]);
            var textObj = fpdf_edit.FPDFPageObjNewTextObj(Page.Document.Document, TextFont, TextSize);
            var success = fpdf_edit.FPDFTextSetText(textObj, ref value[0])== 1;
            if (!success) throw new NotImplementedException("Set text fail");
            //text fill color
            if (FillColor != null)
                fpdf_edit.FPDFPageObjSetFillColor(textObj, FillColor.Value.R, FillColor.Value.G, FillColor.Value.B, FillColor.Value.A);
            //text stroke color
            if(StrokeColor != null)
                fpdf_edit.FPDFPageObjSetStrokeColor(textObj, StrokeColor.Value.R, StrokeColor.Value.G, StrokeColor.Value.B, StrokeColor.Value.A);
            fpdf_annot.FPDFAnnotAppendObject(Annotation, textObj);
        }
    }
}
