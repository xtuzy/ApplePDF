using ApplePDF.PdfKit;
using Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit
{
    public class PdfDocument : IPdfDocument
    {
        public PdfDocument(string? filePath, string? password)
        {
            Document = new iOSPdfKit.PdfDocument(new NSUrl(filePath, false));
            if (Document.IsLocked)
            {
                Document.Unlock(password);
            }
        }

        public PdfDocument(Stream stream, string password)
        {
            Document = new iOSPdfKit.PdfDocument(NSData.FromStream(stream));
            if (Document.IsLocked)
            {
                Document.Unlock(password);
            }
        }

        private PdfDocument()
        {
            Document = new iOSPdfKit.PdfDocument();
        }

        /// <summary>
        /// For create a new doc, not from stream or file.
        /// </summary>
        public static PdfDocument Create()
        {
            return new PdfDocument();
        }

        public PlatformPdfDocument? Document { get; private set; }

        public bool IsEncrypted => Document.IsEncrypted;

        public bool IsLocked => Document.IsLocked;

        public int MajorVersion => (int)Document.MajorVersion;

        public int MinorVersion => (int)Document.MinorVersion;

        public PdfOutline? OutlineRoot { get => new PdfOutline(Document.OutlineRoot); set => Document.OutlineRoot = value.Outline; }

        public int PageCount => (int)Document.PageCount;

        public event EventHandler DidUnlock;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ExchangePages(int indexA, int indexB)
        {
            throw new NotImplementedException();
        }

        public PdfPage? GetPage(int index)
        {
            return new PdfPage(this, index);
        }

        public int GetPageIndex(PdfPage page)
        {
            return page.PageIndex;
        }

        public PdfSelection? GetSelection(PdfPage startPage, Point startPoint, PdfPage endPage, Point endPoint)
        {
            throw new NotImplementedException();
        }

        public PdfSelection? GetSelection(PdfPage startPage, int startCharIndex, PdfPage endPage, int endCharIndex)
        {
            throw new NotImplementedException();
        }

        public void InsertPage(PdfPage page, int index)
        {
            Document.InsertPage(page.Page, index);
        }

        public PdfOutline? OutlineItem(PdfSelection selection)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(int index)
        {
            throw new NotImplementedException();
        }

        public bool Unlock(string password)
        {
            throw new NotImplementedException();
        }
    }
}
