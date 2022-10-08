using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public interface IPdfSelection : IDisposable
    {
        /// <summary>
        /// 具有单个字符或字符范围的属性的字符串.
        /// </summary>
        //NSAttributedString AttributedString { get; }
        List<PdfAttributedString> AttributedString { get; }
        //IntPtr ClassHandle { get; }
        /// <summary>
        /// iOS:Gets or sets the color with which to draw the selection.
        /// </summary>
        Color Color { get; set; }
        /// <summary>
        /// iOS:Returns the pages that are in the selection.
        /// </summary>
        IEnumerable<PdfPage> Pages { get; }
        /// <summary>
        /// iOS: Gets the text of the selection.
        /// </summary>
        string Text { get; }
        /// <summary>
        /// iOS:Adds the provided selection to this selection.
        /// </summary>
        /// <param name="selection"></param>
        void AddSelection(PdfSelection selection);
        /// <summary>
        /// iOS:Adds the provided selections to this selection.
        /// </summary>
        /// <param name="selections"></param>
        void AddSelections(PdfSelection[] selections);
        //NSObject Copy(NSZone zone);
        //void Draw(PdfPage page, bool active);
        //void Draw(PdfPage page, PdfDisplayBox box, bool active);
        /// <summary>
        /// iOS:Adds the specified array of selections to the receiving selection.
        /// </summary>
        /// <param name="succeed"></param>
        void ExtendSelectionAtEnd(int succeed);
        /// <summary>
        /// iOS:Extends the selection from its start toward the beginning of the document.
        /// </summary>
        /// <param name="precede"></param>
        void ExtendSelectionAtStart(int precede);
        void ExtendSelectionForLineBoundaries();
        /// <summary>
        /// Returns the bounds of the selection on the specified page.
        /// Discussion:
        /// The selection rectangle is given in page space.
        /// Page space is a 72 dpi coordinate system with the origin at the lower-left corner of the current page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        RectangleF GetBoundsForPage(PdfPage page);
        int GetNumberOfTextRanges(PdfPage page);
        //NSRange GetRange(nuint index, PdfPage page);
        /// <summary>
        /// If you call this method on a PDFSelection object that represents a paragraph, for example, selectionsByLine returns an array that contains one PDFSelection object for each line of text in the paragraph.
        /// </summary>
        /// <returns></returns>
        PdfSelection[] SelectionsByLine();
    }
}