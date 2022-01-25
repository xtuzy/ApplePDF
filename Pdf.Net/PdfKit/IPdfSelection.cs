using System;
using System.Drawing;
namespace Pdf.Net.PdfKit
{
    public interface IPdfSelection:IDisposable
    {
        //NSAttributedString AttributedString { get; }
        //IntPtr ClassHandle { get; }
        Color Color { get; set; }
        PdfPage[] Pages { get; }
        string Text { get; }

        void AddSelection(PdfSelection selection);
        void AddSelections(PdfSelection[] selections);
        //NSObject Copy(NSZone zone);
        //void Draw(PdfPage page, bool active);
        //void Draw(PdfPage page, PdfDisplayBox box, bool active);
        void ExtendSelectionAtEnd(int succeed);
        void ExtendSelectionAtStart(int precede);
        void ExtendSelectionForLineBoundaries();
        Rectangle GetBoundsForPage(PdfPage page);
        int GetNumberOfTextRanges(PdfPage page);
        //NSRange GetRange(nuint index, PdfPage page);
        PdfSelection[] SelectionsByLine();
    }
}