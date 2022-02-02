
using Pdf.Net.PdfKit.Annotation;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pdf.Net.PdfKit
{
    public abstract class PdfAnnotation : IDisposable
    {
        //参考:https://github.com/LibreOffice/core/blob/92cba30d5ce45e4f4a9516a80c9fe9915add6905/include/vcl/filter/PDFiumLibrary.hxx
        public static string ConstDictionaryKeyTitle = "T";
        public static string ConstDictionaryKeyContents = "Contents";
        public static string ConstDictionaryKeyPopup = "Popup";
        public static string ConstDictionaryKeyModificationDate = "M";
        public static string ConstDictionaryKeyInteriorColor = "IC";
        public Color? FillColor;
        public Color? StrokeColor;

        internal PdfAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
        {
            this.Page = page;
            this.Annotation = annotation;
            this.AnnotationType = type;
            this.Index = index;
            // 颜色
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var success = fpdf_annot.FPDFAnnotGetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, ref R, ref G, ref B, ref A) == 1;
            if (success)
                AnnotColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            else
                Debug.WriteLine("Get Annotation Color fail.");

            //颜色
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var o = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                if (o != null)
                {
                    var objectType = fpdf_edit.FPDFPageObjGetType(o);
                    success = fpdf_edit.FPDFPageObjGetFillColor(o, ref R, ref G, ref B, ref A) == 1;
                    if (success)
                        FillColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                    else
                        Debug.WriteLine("No fill color");
                    success = fpdf_edit.FPDFPageObjGetStrokeColor(o, ref R, ref G, ref B, ref A) == 1;

                    if (success)
                        StrokeColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                    else
                        Debug.WriteLine("No stroke color");
                }
            }
            // 位置
            var position = new FS_RECTF_();
            success = fpdf_annot.FPDFAnnotGetRect(Annotation, position) == 1;
            if (!success) Debug.WriteLine("Get Annotation Position fail.");
            Position = new RectangleF(position.Left, position.Top, position.Right - position.Left, position.Top - position.Bottom);
        }

        public PdfAnnotation(PdfAnnotationSubtype type)
        {
            AnnotationType = type;
        }

        public FpdfAnnotationT Annotation { get; protected set; }
        public int Index { get; protected set; }
        public PdfPage Page { get; protected set; }
        public PdfAnnotationSubtype AnnotationType { get; protected set; }
        #region 用户设置
        /// <summary>
        /// Default color is yellow.
        /// </summary>
        public Color? AnnotColor { get; set; }
        public RectangleF Position { get; set; }
        #endregion 用户设置
        internal virtual void AddToPage(PdfPage page)
        {
            this.Page = page;
            //创建注释，并将注释的属性都添加到页面
            var anno = fpdf_annot.FPDFPageCreateAnnot(page.Page, (int)this.AnnotationType);
            this.Annotation = anno;
            var index = fpdf_annot.FPDFPageGetAnnotIndex(page.Page, this.Annotation);
            this.Index = index;
            bool success = false;
            //颜色
            if (AnnotColor != null)
            {
                success = fpdf_annot.FPDFAnnotSetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, AnnotColor.Value.R, AnnotColor.Value.G, AnnotColor.Value.B, AnnotColor.Value.A) == 1;
                if (!success) throw new NotImplementedException();
            }
            //位置
            fpdf_annot.FPDFAnnotSetRect(Annotation, new FS_RECTF_()
            {
                Left = Position.Left,
                Top = Position.Top,
                Right = Position.Right,
                Bottom = Position.Bottom
            });
            //Flag?
            success = fpdf_annot.FPDFAnnotSetFlags(Annotation, 4) == 1;
            if (!success) new NotImplementedException();
        }

        public void Dispose()
        {
            Page = null;
            if (Annotation != null)
                fpdf_annot.FPDFPageCloseAnnot(Annotation);
            Annotation = null;
        }
    }
}