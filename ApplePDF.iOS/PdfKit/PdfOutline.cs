namespace ApplePDF.PdfKit
{
    public class PdfOutline : IPdfOutline
    {
        internal PdfOutline(iOSPdfKit.PdfOutline outline)
        {
            Outline = outline;
        }

        public iOSPdfKit.PdfOutline Outline { get; private set; }
        public PdfAction Action { get => new PdfActionGoTo(Document, this); set => throw new System.NotImplementedException(); }

        public int ChildrenCount => throw new System.NotImplementedException();

        public PdfDestination Destination { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public PdfDocument? Document => throw new System.NotImplementedException();

        public int Index => throw new System.NotImplementedException();

        public bool IsOpen { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Label { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public PdfOutline Parent => throw new System.NotImplementedException();

        public PdfOutline Child(int index)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void InsertChild(PdfOutline child, int index)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFromParent()
        {
            throw new System.NotImplementedException();
        }
    }
}