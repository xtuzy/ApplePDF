using System.Collections.Generic;

namespace ApplePDF.PdfKit
{
    public class PdfOutline : IPdfOutline
    {
        internal PdfOutline(PdfDocument doc, iOSPdfKit.PdfOutline outline)
        {
            Document = doc;
            Outline = outline;
        }

        public iOSPdfKit.PdfOutline Outline { get; private set; }
        public PdfAction Action { get => new PdfActionGoTo(Document, this); set => Outline.Action = value.Action; }

        public int ChildrenCount => (int)Outline.ChildrenCount;

        public PdfDestination Destination { get => new PdfDestination(Document, Outline.Destination); set => Outline.Destination = value.Destination; }

        public PdfDocument? Document { get; private set; }

        public int Index => (int)Outline.Index;

        public bool IsOpen { get => Outline.IsOpen; set => Outline.IsOpen = value; }
        public string Label { get => Outline.Label; set => Outline.Label = value; }

        public PdfOutline Parent => new PdfOutline(Document, Outline.Parent);

        public List<PdfOutline> Children { get; private set; } = new List<PdfOutline>();

        internal void LoadChildrenBookmarks()
        {
            int childCount = ChildrenCount;
            if (childCount == 0)
                return;

            for (int index = 0; index < childCount; index++)
            {
                var b = this.Child(index);
                Children.Add(b);
                b.LoadChildrenBookmarks();
            }
        }

        public PdfOutline Child(int index)
        {
            return new PdfOutline(Document, Outline.Child(index));
        }

        public void Dispose()
        {
            Document = null;
            Outline?.Dispose();
            Outline = null;
            foreach(PdfOutline b in Children)
            {
                b.Dispose();
            }
        }

        public void InsertChild(PdfOutline child, int index)
        {
            Outline.InsertChild(child.Outline, index);
        }

        public void RemoveFromParent()
        {
            Outline.RemoveFromParent();
        }
    }
}