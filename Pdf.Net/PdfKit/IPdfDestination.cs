
using System;
using System.Drawing;

namespace Pdf.Net.PdfKit
{
    public interface IPdfDestination:IDisposable
    {
        //IntPtr ClassHandle { get; }
        int PageIndex { get; }
        PointF Point { get; }
        float Zoom { get; set; }

        //NSComparisonResult Compare(PdfDestination destination);
        //NSObject Copy(NSZone zone);
    }
}