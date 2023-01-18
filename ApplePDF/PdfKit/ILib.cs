using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit
{
    public interface ILib
    {
        PdfDocument LoadPdfDocument(Stream stream, string password);
        PdfDocument LoadPdfDocument(byte[] bytes, string password);
        PdfDocument LoadPdfDocument(string filePath, string password);
        /// <summary>
        /// 合并两个doc到一个新doc.
        /// </summary>
        /// <param name="firstDoc"></param>
        /// <param name="secondDoc"></param>
        /// <param name="stream"></param>
        void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream);
        /// <summary>
        /// 切割doc中的一部分页面保存到一个新doc.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fromePageIndex"></param>
        /// <param name="toPageIndex"></param>
        /// <param name="stream"></param>
        void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream);
    }
}
