
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class Pdfium
    {
        static readonly object @lock = new object();

        static Pdfium instance;
        public static Pdfium Instance
        {
            get
            {

                if (instance == null)
                {
                    lock (@lock)
                    {
                        if (instance == null)
                        {
                            instance = new Pdfium();
                        }
                    }
                }

                return instance;
            }
        }

        public Pdfium()
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

        public PdfDocument LoadPdfDocument(string filePath, string password)
        {
            lock (@lock)
            {
                return new PdfDocument(filePath, password);
            }
        }

        public void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream)
        {

            var pageCountOne = firstDoc.PageCount;

            var success = fpdf_ppo.FPDF_ImportPages(
                              firstDoc.Document,
                              secondDoc.Document,
                              null,
                              pageCountOne) == 1;

            if (!success)
            {
                throw new Exception("failed to merge files");
            }

            Save(firstDoc, stream);
        }

        public void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream)
        {
            var pageRange = $"{fromePageIndex + 1} - {toPageIndex + 1}";
            using (var childDoc = new PdfDocument())
            {

                var success = fpdf_ppo.FPDF_ImportPages(
                                  childDoc.Document,
                                  doc.Document,
                                  pageRange,
                                  0) == 1;

                if (!success)
                {
                    throw new Exception("failed to split file");
                }

                Save(childDoc, stream);
            }
        }

        //TODO:find a way save big file by use small memory.
        public bool Save(PdfDocument doc, Stream stream, PdfSaveFlag saveFlag = PdfSaveFlag.NoIncremental)
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

        public bool Save(PdfDocument doc, string filePath, PdfSaveFlag saveFlag = PdfSaveFlag.NoIncremental)
        {
            bool success;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                success = Save(doc, stream, saveFlag);
            }
            return success;
        }

        public void DestoryLibrary()
        {
            lock (@lock)
            {
                fpdfview.FPDF_DestroyLibrary();
            }
        }
    }
}