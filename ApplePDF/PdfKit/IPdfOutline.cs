using System;
using System.Collections.Generic;

namespace ApplePDF.PdfKit
{
    //
    // Summary:
    //     A node in a logical outline of a PDF document.
    //
    // Remarks:
    //     To be added.
    public interface IPdfOutline:IDisposable
    {
        /// <summary>
        /// outline的行为, 如跳转到页
        /// #iOS
        /// </summary>
        PdfAction Action { get; set; }
        int ChildrenCount { get; }
        //IntPtr ClassHandle { get; }
        /// <summary>
        /// outline所在的位置
        /// #iOS
        /// </summary>
        PdfDestination Destination { get; set; }
        PdfDocument? Document { get; }
        int Index { get; }
        bool IsOpen { get; set; }
        string Label { get; set; }
        PdfOutline Parent { get; }
        /// <summary>
        /// 一次性获取所有Child
        /// #ApplePDF
        /// </summary>
        List<PdfOutline> Children { get; }

        PdfOutline Child(int index);
        void InsertChild(PdfOutline child, int index);
        void RemoveFromParent();
    }
}