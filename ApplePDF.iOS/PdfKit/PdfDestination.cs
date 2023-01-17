using System.Drawing;
using System.Reflection.Metadata;

namespace ApplePDF.PdfKit
{
    public class PdfDestination : IPdfDestination
    {
        private PdfDocument document;

        public PdfDestination(PdfDocument document, PdfAction action)
        {
            this.document = document;
            iOSPdfKit.PdfDestination destination = (action.Action as iOSPdfKit.PdfActionGoTo)?.Destination;
            Destination = destination;
        }

        /// <summary>
        /// <br/>#iOS Api
        /// </summary>
        public iOSPdfKit.PdfDestination Destination { get; private set; }

        public int PageIndex => (int)document.Document.GetPageIndex(Destination.Page);
        public PdfPage Page => PdfPage.Create(document, Destination.Page);

        public PointF Point => throw new System.NotImplementedException();

        public float Zoom { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}