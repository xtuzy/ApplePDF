using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfSelection : IPdfSelection, ICloneable
    {
        public PdfSelection(PdfPage page, PdfRectangleF rectangle)
        {
            Selections = new Dictionary<PdfRectangleF, PdfPage>();
            Selections.Add(rectangle, page);
        }

        internal Dictionary<PdfRectangleF, PdfPage> Selections { get; private set; }
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

        public List<PdfAttributedString> AttributedString
        {
            get
            {
                var list = new List<PdfAttributedString>();
                //TODO:分析选择区域内有哪些字符
                foreach (var selection in Selections)
                {
                    var page = selection.Value;
                    var xTolerance = 5;
                    var yTolerance = 5;
                    var firstSelectedCharIndex = fpdf_text.FPDFTextGetCharIndexAtPos(page.TextPage, selection.Key.Left, selection.Key.Top, xTolerance, yTolerance);
                    if(firstSelectedCharIndex == -1)//没找到，扩大范围重试
                        firstSelectedCharIndex = fpdf_text.FPDFTextGetCharIndexAtPos(page.TextPage, selection.Key.Left, selection.Key.Top, 10, 10);
                    var lastSelectedCharIndex = fpdf_text.FPDFTextGetCharIndexAtPos(page.TextPage, selection.Key.Right, selection.Key.Bottom, xTolerance, yTolerance);
                    if(lastSelectedCharIndex == -1)
                        lastSelectedCharIndex = fpdf_text.FPDFTextGetCharIndexAtPos(page.TextPage, selection.Key.Right, selection.Key.Bottom, 10, 10);
                    if (firstSelectedCharIndex == -1 || lastSelectedCharIndex == -1)
                        throw new ArgumentException($"Can't find char index in start and end of Rectangle {selection.Key}");
                    var count = lastSelectedCharIndex - firstSelectedCharIndex;
                    var rects = page.GetCharactersBounds(firstSelectedCharIndex, count);
                    foreach (var rect in rects)
                    {
                        firstSelectedCharIndex = fpdf_text.FPDFTextGetCharIndexAtPos(page.TextPage, rect.Left, rect.Top, 2, 2);
                        var str = GetSelectTextInPage(page, rect);
                        list.Add(new PdfAttributedString(str, rect, firstSelectedCharIndex));
                    }
                }
                return list;
            }
        }

        string GetSelectTextInPage(PdfPage page, PdfRectangleF rectangle)
        {
            ushort[] buffer = new ushort[1];
            int canCharactersWritten;
            //获取字符数
            canCharactersWritten = fpdf_text.FPDFTextGetBoundedText(page.TextPage, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, ref buffer[0], 0);
            if (canCharactersWritten > 0)
            {
                buffer = new ushort[canCharactersWritten];
                var finalCharactersWritten = fpdf_text.FPDFTextGetBoundedText(page.TextPage, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, ref buffer[0], buffer.Length);
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

        public PdfRectangleF GetBoundsForPage(PdfPage page)
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

        public object Clone()
        {
            var first = Selections.First();
            var selection = new PdfSelection(first.Value, first.Key);
            foreach (var child in Selections)
            {
                if (selection.Selections.ContainsKey(child.Key))
                {
                    continue;
                }
                else
                {
                    selection.AddSelection(new PdfSelection(child.Value, child.Key));
                }
            }
            return selection;
        }
    }
}