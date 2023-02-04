using PDFiumCore;
using System;
using System.Drawing;
using System.IO;

namespace ApplePDF.PdfKit
{
    public class PdfDocument : IPdfDocument
    {
        private static readonly object @lock = new object();
        private FpdfDocumentT document;
        private PdfOutline outlineRoot;
        private int streamId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocument"/> class.
        /// </summary>
        /// <param name="filePath">path to file</param>
        /// <param name="password">null or string</param>
        public PdfDocument(string? filePath, string? password)
        {
            lock (@lock)
            {
                if (filePath == null)
                {
                    throw new ArgumentNullException(nameof(filePath), "FilePath should't be null.");
                }
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(nameof(filePath), $"{filePath} not exist.");
                }
                document = fpdfview.FPDF_LoadDocument(filePath, password);
                if (document == null)
                    throw new Exception("Can't load this doc");
            }
        }

        public PdfDocument(Stream stream, string password)
        {
            lock (@lock)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException(nameof(stream), "Stream should't be null.");
                }

                // 参考https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfFile.cs实现
                this.streamId = StreamManager.Register(stream);
                this.document = fpdfview.FPDF_LoadDocument(stream, password, this.streamId);
                if (document == null)
                    throw new Exception("Can't load this doc.");//TODO:Find Reason
            }
        }

        /// <summary>
        /// For create a new doc, not from stream or file.
        /// </summary>
        internal PdfDocument(PlatformPdfDocument pdfDocument)
        {
            document = pdfDocument;
        }

        /// <inheritdoc/>
        public FpdfDocumentT Document
        {
            get
            {
                if (document == null)
                    throw new ObjectDisposedException(nameof(document));
                return document;
            }
        }

        public bool IsEncrypted => throw new NotImplementedException();

        public bool IsLocked => throw new NotImplementedException();

        int version = 0;

        public int MajorVersion
        {
            get
            {
                lock (@lock)
                {
                    if (version == 0)
                    {
                        var success = fpdfview.FPDF_GetFileVersion(Document, ref version) == 1;
                        if (!success)
                            version = 17;
                    }
                    return version / 10;
                }
            }
        }

        public int MinorVersion
        {
            get
            {
                lock (@lock)
                {
                    if (version == 0)
                    {
                        var mVersion = MajorVersion;
                    }
                    return version % 10;
                }
            }
        }

        /// <summary>
        /// get page size by not open page.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SizeF GetPageSize(int pageIndex)
        {
            var size = new FS_SIZEF_();
            // var result = fpdfview.FPDF_GetPageSizeByIndex(Document, pageIndex, ref width, ref height);
            var result = fpdfview.FPDF_GetPageSizeByIndexF(Document, pageIndex, size);
            if (result == 0)
            {
                return new SizeF(0, 0);
            }
            else
            {
                return new SizeF(size.Width, size.Height);
            }
        }

        public PdfOutline OutlineRoot
        {
            get
            {
                lock (@lock)
                {
                    if (outlineRoot == null)
                    {
                        var bookmark = fpdf_doc.FPDFBookmarkGetFirstChild(this.Document, null);
                        if (bookmark != null)
                        {
                            outlineRoot = new PdfOutline(this, bookmark);
                            outlineRoot.LoadChildren();
                        }
                    }

                    return outlineRoot;
                }
            }
            set => throw new NotImplementedException();
        }

        public int PageCount
        {
            get
            {
                lock (@lock)
                {
                    return fpdfview.FPDF_GetPageCount(Document);
                }
            }
        }

        public event EventHandler DidUnlock;

        public void ExchangePages(int indexA, int indexB)
        {
            throw new NotImplementedException();
        }

        public PdfPage GetPage(int index)
        {
            if (index < 0 || index >= PageCount)
                throw new ArgumentOutOfRangeException($"Page Index should > 0 and < {PageCount}");
            return new PdfPage(this, index);
        }

        public int GetPageIndex(PdfPage page)
        {
            return page.PageIndex;
        }

        public PdfSelection GetSelection(PdfPage startPage, Point startPoint, PdfPage endPage, Point endPoint)
        {
            throw new NotImplementedException();
        }

        public PdfSelection GetSelection(PdfPage startPage, int startCharIndex, PdfPage endPage, int endCharIndex)
        {
            throw new NotImplementedException();
        }

        public PdfOutline OutlineItem(PdfSelection selection)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(int index)
        {
            fpdf_edit.FPDFPageDelete(Document, index);
        }

        public bool Unlock(string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建新的Page到文档
        /// </summary>
        /// <param name="index"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public PdfPage CreatePage(int index, int w, int h)
        {
            var platformPage = fpdf_edit.FPDFPageNew(Document, index, w, h);
            return new PdfPage(this, index, platformPage);
        }

        /// <summary>
        /// Close this pdf
        /// </summary>
        public void Dispose()
        {
            lock (@lock)
            {
                // 关闭文档
                fpdfview.FPDF_CloseDocument(document);
                document = null;
                if (outlineRoot != null)
                    outlineRoot.Dispose();
                outlineRoot = null;

                // 释放流
                var stream = StreamManager.Get(streamId);
                if (stream != null)
                    stream.Dispose();
                StreamManager.Unregister(streamId);
            }
        }
    }
}
