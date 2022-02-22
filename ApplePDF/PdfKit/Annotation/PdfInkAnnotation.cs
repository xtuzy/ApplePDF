using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit.Annotation
{
    public class PdfInkAnnotation : PdfAnnotation
    {
        public PdfInkAnnotation(PdfAnnotationSubtype type) : base(type)
        {
        }

        internal PdfInkAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
        }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);
        }

        public int AddInk(List<PointF> ink)
        {
            FS_POINTF_[] points = new FS_POINTF_[ink.Count];
            for (var index = 0; index < ink.Count; index++)
            {
                points[index] = new FS_POINTF_() { X = ink[index].X, Y = ink[index].Y };
            }
            var success = fpdf_annot.FPDFAnnotAddInkStroke(Annotation, points[0], (ulong)points.LongLength);
            if (success == -1)
                throw new NotImplementedException();
            else
            {
                Inks.Add(ink);
                return success;
            }
        }

        public void RemoveAllInk()
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            Inks.Clear();
        }

        public void RemoveInk(List<PointF> ink)
        {
            fpdf_annot.FPDFAnnotRemoveInkList(Annotation);
            if (!Inks.Contains(ink)) throw new NotImplementedException();
            Inks.Remove(ink);
            foreach (var i in Inks)
            {
                AddInk(i);
            }
        }

        public List<List<PointF>> Inks;

        public void GetInks()
        {
            var count = fpdf_annot.FPDFAnnotGetInkListCount(Annotation);
            if (count == 0)
                throw new NotImplementedException("No ink at this annot");
            Inks = new List<List<PointF>>();
            for (int index = 0; index < count; index++)
            {
                var pointCount = fpdf_annot.FPDFAnnotGetInkListPath(Annotation, (uint)index, null, 0);
                FS_POINTF_[] points = new FS_POINTF_[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    points[i] = new FS_POINTF_();
                }
                var success = fpdf_annot.FPDFAnnotGetInkListPath(Annotation, (uint)index, points[0], pointCount) == pointCount;
                if (!success)
                    throw new NotImplementedException();
                var ink = new List<PointF>();
                foreach (var point in points)
                {
                    ink.Add(new PointF(point.X, point.Y));
                }
                Inks.Add(ink);
            }
        }
    }
}
