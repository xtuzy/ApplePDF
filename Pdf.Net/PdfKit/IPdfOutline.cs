using System;

namespace Pdf.Net.PdfKit
{
    //
    // Summary:
    //     A node in a logical outline of a PDF document.
    //
    // Remarks:
    //     To be added.
    public interface IPdfOutline:IDisposable
    {
        PdfAction Action { get; set; }
        int ChildrenCount { get; }
        //IntPtr ClassHandle { get; }
        PdfDestination Destination { get; set; }
        PdfDocument? Document { get; }
        int Index { get; }
        bool IsOpen { get; set; }
        string Label { get; set; }
        PdfOutline Parent { get; }

        PdfOutline Child(int index);
        void InsertChild(PdfOutline child, int index);
        void RemoveFromParent();
    }
}