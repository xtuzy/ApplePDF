using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfSelection : IPdfSelection
    {
        public PdfSelection(PdfPage page, RectangleF rectangle)
        {
            Selections = new Dictionary<RectangleF, PdfPage>();
            Selections.Add(rectangle, page);
        }

        internal Dictionary<RectangleF, PdfPage> Selections { get; private set; }
        public Color Color { get; set; }
        public IEnumerable<PdfPage> Pages { get => Selections.Values; }
        public string Text
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (var selection in Selections)
                    s.Append(GetSelectTextInPage(selection.Value, selection.Key));
                return s.ToString();
            }
        }

        string GetSelectTextInPage(PdfPage page, RectangleF rectangle)
        {
            ushort[] buffer = null;
            int canCharactersWritten;
            //获取字符数
            canCharactersWritten = fpdf_text.FPDFTextGetBoundedText(page.TextPage, rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom, ref buffer[0], 0);
            if (canCharactersWritten > 0)
            {
                buffer = new ushort[canCharactersWritten];
                var finalCharactersWritten = fpdf_text.FPDFTextGetBoundedText(page.TextPage, rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom, ref buffer[0], buffer.Length);
                if (finalCharactersWritten == canCharactersWritten)
                {
                    //参考:https://stackoverflow.com/a/274207/13254773
                    string result;
                    unsafe
                    {
                        fixed (ushort* dataPtr = &buffer[0])
                        {
                            result = new string((char*)dataPtr, 0, finalCharactersWritten);
                        }
                    }
                    return result;
                }
                else
                    throw new NotImplementedException("Text buffer length is not inconsistent of input and return when GetSelection, this is ApplePDF not do right.");
            }
            else
                return string.Empty;
        }

        public void AddSelection(PdfSelection selection)
        {
            foreach (var childSelection in selection.Selections)
                Selections.Add(childSelection.Key, childSelection.Value);
        }

        public void AddSelections(PdfSelection[] selections)
        {
            foreach (var selection in selections)
                foreach (var childSelection in selection.Selections)
                    Selections.Add(childSelection.Key, childSelection.Value);
        }

        public void ExtendSelectionAtEnd(int succeed)
        {
            throw new NotImplementedException();
        }

        public void ExtendSelectionAtStart(int precede)
        {
            throw new NotImplementedException();
        }

        public void ExtendSelectionForLineBoundaries()
        {
            throw new NotImplementedException();
        }

        public RectangleF GetBoundsForPage(PdfPage page)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfTextRanges(PdfPage page)
        {
            throw new NotImplementedException();
        }

        public PdfSelection[] SelectionsByLine()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Selections.Clear();
            Selections = null;
        }
    }
}