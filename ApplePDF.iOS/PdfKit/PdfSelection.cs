using ApplePDF.Extensions;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public class PdfSelection : IPdfSelection
    {
        public PdfSelection(PdfPage page, PdfRectangleF rectangle)
        {
            Selection = page.Page.GetSelection(rectangle.ToCGRect());
        }

        public PlatformPdfSelection Selection { get; private set; }
        internal PdfSelection(PlatformPdfSelection selection)
        {
            Selection = selection;
        }

        public List<PdfAttributedString> AttributedString => PdfAttributedString.Phrase(Selection.AttributedString);

        public Color Color { get => Selection.Color.ToColor(); set => Selection.Color = value.ToUIColor(); }

        public IEnumerable<PdfPage> Pages => throw new System.NotImplementedException();

        public string Text => Selection.Text;

        public void AddSelection(PdfSelection selection)
        {
            Selection.AddSelection(selection.Selection);
        }

        public void AddSelections(PdfSelection[] selections)
        {
            PlatformPdfSelection[] pdfSelections = new PlatformPdfSelection[selections.Length];
            selections.CopyTo(pdfSelections, 0);
            Selection.AddSelections(pdfSelections);
        }

        public void Dispose()
        {
            Selection.Dispose();
        }

        public void ExtendSelectionAtEnd(int succeed)
        {
            throw new System.NotImplementedException();
        }

        public void ExtendSelectionAtStart(int precede)
        {
            throw new System.NotImplementedException();
        }

        public void ExtendSelectionForLineBoundaries()
        {
            throw new System.NotImplementedException();
        }

        public PdfRectangleF GetBoundsForPage(PdfPage page)
        {
            return Selection.GetBoundsForPage(page.Page).ToPdfRectangleF();
        }

        public int GetNumberOfTextRanges(PdfPage page)
        {
            return (int)Selection.GetNumberOfTextRanges(page.Page);
        }

        public PdfSelection[] SelectionsByLine()
        {
            var selections = Selection.SelectionsByLine();
            PdfSelection[] result = new PdfSelection[selections.Length];
            for(int i =0; i< selections.Length; i++)
            {
                result[i] = new PdfSelection(selections[i]);
            }
            return result;
        }
    }
}