using ApplePDF.Extensions;
using Foundation;
using System;
using System.Drawing;
using System.IO;

namespace ApplePDF.PdfKit
{
    public class PdfDocument : IPdfDocument
    {
        public PdfDocument(string? filePath, string? password)
        {
            document = new iOSPdfKit.PdfDocument(new NSUrl(filePath, false));
            if (Document.IsLocked)
            {
                Document.Unlock(password);
            }
        }

        public PdfDocument(Stream stream, string password)
        {
            var data = NSData.FromStream(stream);
            document = new iOSPdfKit.PdfDocument(data);
            if (Document.IsLocked)
            {
                Document.Unlock(password);
            }
        }

        public PdfDocument(byte[] bytes, string password)
        {
            var data = NSData.FromArray(bytes);
            document = new iOSPdfKit.PdfDocument(data);
            if (Document.IsLocked)
            {
                Document.Unlock(password);
            }
        }

        internal PdfDocument(PlatformPdfDocument pdfDocument)
        {
            document = pdfDocument;
        }

        PlatformPdfDocument document;
        public PlatformPdfDocument? Document 
        { 
            get
            {
                if (document == null)
                    throw new ObjectDisposedException(nameof(Document));
                return document;
            }
        }

        public bool IsEncrypted => Document.IsEncrypted;

        public bool IsLocked => Document.IsLocked;

        public int MajorVersion => (int)Document.MajorVersion;

        public int MinorVersion => (int)Document.MinorVersion;

        public PdfOutline? OutlineRoot
        {
            get
            {
                var root = new PdfOutline(this, Document.OutlineRoot);
                root.LoadChildrenBookmarks();
                return root;
            }
            set => Document.OutlineRoot = value.Outline;
        }

        public int PageCount => (int)Document.PageCount;

        public event EventHandler DidUnlock;

        public void Dispose()
        {
            document?.Dispose();
            document = null;
        }

        public void ExchangePages(int indexA, int indexB)
        {
            Document.ExchangePages(indexA, indexB);
        }

        public PdfPage? GetPage(int index)
        {
            if (index < 0 || index >= PageCount)
                throw new ArgumentOutOfRangeException($"Page Index should > 0 and < {PageCount}");
            return new PdfPage(this, index);
        }

        public int GetPageIndex(PdfPage page)
        {
            return (int)Document.GetPageIndex(page.Page);
        }

        public PdfSelection? GetSelection(PdfPage startPage, Point startPoint, PdfPage endPage, Point endPoint)
        {
            return new PdfSelection(Document.GetSelection(startPage.Page, startPoint.ToCGPoint(), endPage.Page, endPoint.ToCGPoint()));
        }

        public PdfSelection? GetSelection(PdfPage startPage, int startCharIndex, PdfPage endPage, int endCharIndex)
        {
            return new PdfSelection(Document.GetSelection(startPage.Page, startCharIndex, endPage.Page, endCharIndex));
        }

        public void InsertPage(PdfPage page, int index)
        {
            Document.InsertPage(page.Page, index);
        }

        public PdfOutline? OutlineItem(PdfSelection selection)
        {
            return new PdfOutline(this, Document.OutlineItem(selection.Selection));
        }

        public void RemovePage(int index)
        {
            Document.RemovePage(index);
        }

        public bool Unlock(string password)
        {
            return Document.Unlock(password);
        }

        public PdfPage CreatePage(int index, int w, int h)
        {
            var platformPage = new PlatformPdfPage();
            Document.InsertPage(platformPage, index);
            return new PdfPage(this, platformPage);
        }
    }
}
