
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdf.Net.PdfKit
{
    /// <summary>
    /// A PDFOutline object is an element in a tree-structured hierarchy that can represent the structure of a PDF document.
    /// <br/>
    /// An outline is an optional component of a PDF document, useful for viewing the structure of the document and for navigating within it.
    /// Outlines are created by the document’s author.If you represent a PDF document outline using outline objects, the root of the hierarchy is obtained from the PDF document itself.This root outline is not visible and serves merely as a container for the visible outlines.
    /// </summary>
    public class PdfOutline : IPdfOutline
    {
        private static readonly object @lock = new object();
        private PdfDocument document;
        private FpdfBookmarkT outline;
        PdfAction action;
        PdfDestination destination;
        string lable = string.Empty;

        public FpdfBookmarkT Outline => outline;

        public List<PdfOutline> Children;

        public PdfOutline(PdfDocument document, FpdfBookmarkT bookmark)
        {
            this.document = document;
            this.outline = bookmark;
            Children = new List<PdfOutline>();
        }

        private unsafe string GetBookmarkTitle(FpdfBookmarkT bookmark)
        {
            var length = fpdf_doc.FPDFBookmarkGetTitle(bookmark, IntPtr.Zero, 0);
            var buffer = new byte[length];
            fixed (byte* p = buffer)
            {
                IntPtr ptr = (IntPtr)p;
                // do you stuff here
                fpdf_doc.FPDFBookmarkGetTitle(bookmark, ptr, length);
            }

            var result = Encoding.Unicode.GetString(buffer);
            if (result.Length > 0 && result[result.Length - 1] == 0)
                result = result.Substring(0, result.Length - 1);
            return result;
        }
      
        public PdfAction Action
        {
            get
            {
                if (action == null)
                    action = new PdfActionGoTo(document, this);
                return action;
            }
            set => throw new NotImplementedException();
        }

        public int ChildrenCount => Children.Count;

        public PdfDestination Destination
        {
            get
            {
                if (destination == null)
                    destination = new PdfDestination(document, this);
                return destination;
            }
            set => throw new NotImplementedException();
        }

        public PdfDocument Document => document;

        public int Index => throw new NotImplementedException();

        public bool IsOpen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Label
        {
            get
            {
                if (lable == string.Empty)
                    lable = GetBookmarkTitle(outline);
                return lable;
            }
            set => throw new NotImplementedException();
        }

        public PdfOutline Parent => throw new NotImplementedException();

        public PdfOutline Child(int index)
        {
            return Children[index];
        }

        public void InsertChild(PdfOutline child, int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromParent()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            document = null;
            outline = null;
            if(destination != null)
                destination.Dispose();
            destination = null;
            if(action != null)
                action.Dispose();
            action = null;
            foreach(var child in Children)
            {
                child.Dispose();
            }
            Children = null;
        }
    }
}