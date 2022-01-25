using System;

namespace Pdf.Net.PdfKit
{
    public interface IPdfAction:IDisposable
    {
        //IntPtr ClassHandle { get; }
        string Type { get; set; }

        //NSObject Copy(NSZone zone);
    }
}