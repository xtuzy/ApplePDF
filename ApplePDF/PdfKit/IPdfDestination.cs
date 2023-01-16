
using System;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public interface IPdfDestination:IDisposable
    {
        //IntPtr ClassHandle { get; }
        /// <summary>
        /// Pdfium中可以直接获取页码, 而iOSPdfKit直接获取Page
        /// <br/>#ApplePDF Api
        /// </summary>
        int PageIndex { get; }
        /// <summary>
        /// <br/>#iOS Api
        /// </summary>
        public PdfPage? Page { get; }
        /// <summary>
        /// <br/>#iOS Api
        /// </summary>
        PointF Point { get; }
        /// <summary>
        /// <br/>#iOS Api
        /// </summary>
        float Zoom { get; set; }

        //NSComparisonResult Compare(PdfDestination destination);
        //NSObject Copy(NSZone zone);
    }
}