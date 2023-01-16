using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfDestination : IPdfDestination
    {
        private PdfDocument document;
        private PdfAction action;

        public PdfDestination(PdfDocument document, PdfAction action)
        {
            this.document = document;
            this.action = action;
            var dest = fpdf_doc.FPDFActionGetDest(document.Document, action.Action);
            pageIndex = fpdf_doc.FPDFDestGetDestPageIndex(document.Document, dest);
            int hasx = 0;
            int hasy = 0;
            int haszoom = 0;
            float x = 0f;
            float y = 0f;
            float zoom = 1f;
            fpdf_doc.FPDFDestGetLocationInPage(dest, ref hasx, ref hasy, ref haszoom, ref x, ref y, ref zoom);
            if (hasx != 0 && hasy != 0)
            {
                point = new PointF(x, y);
            }

            if (haszoom != 0)
            {
                this.zoom = zoom;
            }
        }

        public PdfDestination(PdfDocument document,PdfOutline outline)
        {
            this.document = document;
            this.action = action;
            var dest = fpdf_doc.FPDFBookmarkGetDest(document.Document, outline.Outline);
            pageIndex = fpdf_doc.FPDFDestGetDestPageIndex(document.Document, dest);
            int hasx = 0;
            int hasy = 0;
            int haszoom = 0;
            float x = 0f;
            float y = 0f;
            float zoom = 1f;
            fpdf_doc.FPDFDestGetLocationInPage(dest, ref hasx, ref hasy, ref haszoom, ref x, ref y, ref zoom);
            if (hasx != 0 && hasy != 0)
            {
                point = new PointF(x, y);
            }

            if (haszoom != 0)
            {
                this.zoom = zoom;
            }
        }
        int pageIndex = -1;
        public int PageIndex => pageIndex;
        public PdfPage Page => document.GetPage(pageIndex);

        PointF point = PointF.Empty;
        public PointF Point { get => point; }

        float zoom = float.NaN;
        public float Zoom { get => zoom; set => throw new NotImplementedException(); }

        public void Dispose()
        {
            document = null;
            action = null;
        }
    }
}
