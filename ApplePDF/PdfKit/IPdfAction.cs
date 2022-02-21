using System;

namespace ApplePDF.PdfKit
{
    public interface IPdfAction:IDisposable
    {
        //IntPtr ClassHandle { get; }
        string Type { get; set; }

        //NSObject Copy(NSZone zone);
    }
}