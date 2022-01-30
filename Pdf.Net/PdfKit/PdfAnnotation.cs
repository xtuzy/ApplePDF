
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
        internal PdfAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
        {
            this.Page = page;
            this.Annotation = annotation;
            this.AnnotationType = type;
            this.Index = index;
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var success = fpdf_annot.FPDFAnnotGetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, ref R, ref G, ref B, ref A) == 1;
            if (!success) Debug.WriteLine("Get Annotation Color fail.");
            Color = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            var position = new FS_RECTF_();
            success = fpdf_annot.FPDFAnnotGetRect(Annotation, position) == 1;
            if (!success) Debug.WriteLine("Get Annotation Position fail.");
            Position = new RectangleF(position.Left,position.Top,position.Right-position.Left,position.Top-position.Bottom);
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
        public Color Color { get; set; }
        public RectangleF Position { get; set; }
        #endregion 用户设置
        internal virtual void AddToPage(PdfPage page)
        {
            this.Page = page;
            //将注释的属性都添加到页面
            var anno = fpdf_annot.FPDFPageCreateAnnot(page.Page, (int)this.AnnotationType);
            this.Annotation = anno;
            var index = fpdf_annot.FPDFPageGetAnnotIndex(page.Page, this.Annotation);
            this.Index = index;
            var success = fpdf_annot.FPDFAnnotSetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, Color.R, Color.G, Color.B, Color.A) == 1;
            if (success) ;
            fpdf_annot.FPDFAnnotSetRect(Annotation, new FS_RECTF_()
            {
                Left = Position.Left,
                Top = Position.Top,
                Right = Position.Right,
                Bottom = Position.Bottom
            });
            success = fpdf_annot.FPDFAnnotSetFlags(Annotation, 4)==1;
            if(success) ;
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