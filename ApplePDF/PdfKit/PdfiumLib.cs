using PDFiumCore;
using System;
using System.IO;

namespace ApplePDF.PdfKit
{
    public class PdfiumLib : ILib
    {
        static readonly object @lock = new object();

        static PdfiumLib instance;
        public static PdfiumLib Instance
        {
            get
            {

                if (instance == null)
                {
                    lock (@lock)
                    {
                        if (instance == null)
                        {
                            instance = new PdfiumLib();
                        }
                    }
                }

                return instance;
            }
        }

        PdfiumLib()
        {
            InitLibrary();
        }

        /// <summary>
        /// 初始化库,在使用Pdfium前必须执行该初始化.
        /// </summary>
        private void InitLibrary()
        {
            fpdfview.FPDF_InitLibrary();
        }

        public PdfDocument LoadPdfDocument(Stream stream, string password)
        {
            lock (@lock)
            {
                return new PdfDocument(stream, password);
            }
        }

        public PdfDocument LoadPdfDocument(byte[] bytes, string password)
        {
            lock (@lock)
            {
                MemoryStream stream = new MemoryStream(bytes);
                return LoadPdfDocument(stream, password);
            }
        }

        public PdfDocument LoadPdfDocument(string filePath, string password)
        {
            lock (@lock)
            {
                return new PdfDocument(filePath, password);
            }
        }

        /// <summary>
        /// For create a new doc, not from stream or file.
        /// </summary>
        public PdfDocument CreatePdfDocument()
        {
            return new PdfDocument(fpdf_edit.FPDF_CreateNewDocument());
        }

        public void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream)
        {
            Save(Merge(firstDoc, secondDoc), stream);
        }

        public PdfDocument Merge(PdfDocument firstDoc, PdfDocument secondDoc)
        {
            var destPdf = CreatePdfDocument();

            // 插入第一个文档
            var success = fpdf_ppo.FPDF_ImportPages(
                              destPdf.Document,
                              firstDoc.Document,
                              null,
                              0) == 1;
            if (!success)
            {
                throw new Exception("failed to merge files");
            }

            // 插入第二个文档
            var pageCount = firstDoc.PageCount;
            success = fpdf_ppo.FPDF_ImportPages(
                              destPdf.Document,
                              secondDoc.Document,
                              null,
                              pageCount) == 1;
            if (!success)
            {
                throw new Exception("failed to merge files");
            }

            return destPdf;
        }

        public PdfDocument Split(PdfDocument doc, int fromePageIndex, int toPageIndex)
        {
            var pageRange = $"{fromePageIndex + 1} - {toPageIndex + 1}";
            var childDoc = CreatePdfDocument();

            var success = fpdf_ppo.FPDF_ImportPages(
                              childDoc.Document,
                              doc.Document,
                              pageRange,
                              0) == 1;

            if (!success)
            {
                throw new Exception("failed to split file");
            }

            return childDoc;
        }

        public void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream)
        {
            Save(Split(doc, fromePageIndex, toPageIndex), stream);
        }

        //TODO:find a way save big file by use small memory.
        public bool Save(PdfDocument doc, Stream stream, PdfSaveFlag saveFlag)
        {
            lock (@lock)
            {
                var success = fpdfsave.FPDF_SaveAsCopy(doc.Document, stream, saveFlag);

                if (!success)
                {
                    throw new Exception("failed to save the document");
                }
                return success;
            }
        }

        public bool Save(PdfDocument doc, Stream stream) => Save(doc, stream, PdfSaveFlag.NoIncremental);

        public bool Save(PdfDocument doc, string filePath, PdfSaveFlag saveFlag)
        {
            bool success;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                success = Save(doc, stream, saveFlag);
            }
            return success;
        }

        public bool Save(PdfDocument doc, string filePath) => Save(doc, filePath, PdfSaveFlag.NoIncremental);

        public byte[] Save(PdfDocument doc, PdfSaveFlag saveFlag)
        {
            using (var stream = new MemoryStream())
            {
                var success = Save(doc, stream, saveFlag);
                if (success)
                    return stream.ToArray();
                else
                    return null;
            }
        }

        public byte[] Save(PdfDocument doc) => Save(doc, PdfSaveFlag.NoIncremental);

        public void DestoryLibrary()
        {
            lock (@lock)
            {
                fpdfview.FPDF_DestroyLibrary();
            }
        }

        public void Dispose()
        {
            DestoryLibrary();
        }
    }
}