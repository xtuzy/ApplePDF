
using System;
using System.Drawing;

namespace ApplePDF.PdfKit
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