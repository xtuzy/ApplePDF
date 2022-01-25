
using Pdf.Net.PdfKit.Annotation;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdf.Net.PdfKit
{
    public class PdfAnnotation : IDisposable
    {
        private PdfPage page;
        private int index;
        private FpdfAnnotationT annotation;
        PdfAnnotationSubtype annotationType;
        
        internal PdfAnnotation(PdfPage page, int index)
        {
            this.page = page;
            this.index = index;
            annotation = fpdf_annot.FPDFPageGetAnnot(page.Page, index);
            annotationType = (PdfAnnotationSubtype)fpdf_annot.FPDFAnnotGetSubtype(annotation);
        }

        public PdfAnnotation(PdfPage page, PdfAnnotationSubtype type)
        {
            this.page = page;
        }
           
        public FpdfAnnotationT Annotation { get => annotation; internal set => annotation = value; }
        public int Index { get => index; internal set => index = value; }
        public PdfPage Page => page;
        public PdfAnnotationSubtype AnnotationType => annotationType;
        void Color()
        {
            
        }
        public void Dispose()
        {
            page = null;
            if (annotation != null)
                fpdf_annot.FPDFPageCloseAnnot(Annotation);
            annotation = null;
        }
    }
}