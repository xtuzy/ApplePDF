using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit
{
    public interface ILib: IDisposable
    {
        PdfDocument LoadPdfDocument(Stream stream, string password);
        PdfDocument LoadPdfDocument(byte[] bytes, string password);
        PdfDocument LoadPdfDocument(string filePath, string password);
        PdfDocument CreatePdfDocument();
        /// <summary>
        /// 合并两个doc到一个新doc.
        /// 注意该接口中Merge只提供保存到Stream的方法, 其对于Pdfium是方便的, 对于iOS的PdfKit, Merge操作的底层实现是保存到NSData, 其类似byte[], 为避免多余内存开销, 请尝试使用<see cref="PdfKit.PdfKitLib"/>中的相关方法
        /// </summary>
        /// <param name="firstDoc"></param>
        /// <param name="secondDoc"></param>
        void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream);
        /// <summary>
        /// 切割doc中的一部分页面保存到一个新doc.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fromePageIndex"></param>
        /// <param name="toPageIndex"></param>
        /// <param name="stream"></param>
        void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream);
        byte[] Save(PdfDocument doc);
        bool Save(PdfDocument doc, string filePath);
        bool Save(PdfDocument doc, Stream stream);
    }
}
