using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit
{
    public interface IPdfPage_Pdfium : IPdfPage
    {
        /// <summary>
        /// 从0开始
        /// </summary>
        int PageIndex { get; }
        int AnnotationCount { get; }
        SizeF GetSize();
        bool AddText(PdfFont font, float fontSize, string text, double x, double y, double scale = 1);
        bool InsteadText(string oldText, string newText);
        void Draw(IntPtr imageBufferPointer, float xScale, float yScale, int rotate, int renderFlag);
    }
}
