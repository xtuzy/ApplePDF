using System;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public interface IPdfDocument : IDisposable
    {
        //PdfAccessPermissions AccessPermissions { get; }
        //bool AllowsCommenting { get; }
        //bool AllowsContentAccessibility { get; }
        //bool AllowsCopying { get; }
        //bool AllowsDocumentAssembly { get; }
        //bool AllowsDocumentChanges { get; }
        //bool AllowsFormFieldEntry { get; }
        //bool AllowsPrinting { get; }
        //System.IntPtr ClassHandle { get; }
        //IPdfDocumentDelegate Delegate { get; set; }
        /// <summary>
        /// Platform doc object, you can do more operate by using it.
        /// </summary>
        PlatformPdfDocument? Document { get; }
        //NSDictionary? DocumentAttributes { get; set; }
        //NSUrl? DocumentUrl { get; }
        //ClassForAnnotationTypeDelegate? GetClassForAnnotationType { get; set; }
        bool IsEncrypted { get; }
        //bool IsFinding { get; }
        bool IsLocked { get; }
        /// <summary>
        /// 1.3, 1.5, 1.7的整数部分
        /// </summary>
        int MajorVersion { get; }
        /// <summary>
        /// 1.3, 1.5, 1.7的小数部分
        /// </summary>
        int MinorVersion { get; }


        PdfOutline? OutlineRoot { get; set; }
        //Class PageClass { get; }
        int PageCount { get; }
        //System.Type PageType { get; }
        //PdfDocumentPermissions PermissionsStatus { get; }
        //string Text { get; }
        //NSObject? WeakDelegate { get; set; }

        //event System.EventHandler DidBeginDocumentFind;
        //event System.EventHandler DidMatchString;
        event System.EventHandler DidUnlock;
        /// <summary>
        /// iOS中创建platformPdfPage是通过new来创建的, 可以直接操作, 之后再插入文档, 而Pdfium是通过文档创建PlatformPdfPage, 为适配两者因此提供改方法
        /// #ApplePDF
        /// </summary>
        /// <param name="index"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        PdfPage CreatePage(int index, int w, int h);

        //event System.EventHandler FindFinished;
        //event System.EventHandler MatchFound;
        //event System.EventHandler PageFindFinished;
        //event System.EventHandler PageFindStarted;

        //void CancelFind();
        //NSObject Copy(NSZone? zone);
        void ExchangePages(int indexA, int indexB);
        //PdfSelection[] Find(string text, NSStringCompareOptions compareOptions);
        //PdfSelection? Find(string text, PdfSelection? selection, NSStringCompareOptions compareOptions);
        //void FindAsync(string text, NSStringCompareOptions compareOptions);
        //void FindAsync(string[] text, NSStringCompareOptions compareOptions);
        //NSData? GetDataRepresentation();
        //NSData? GetDataRepresentation(NSDictionary options);
        //PdfDocumentAttributes GetDocumentAttributes();
        PdfPage? GetPage(int index);
        int GetPageIndex(PdfPage page);
        PdfSelection? GetSelection(PdfPage startPage, Point startPoint, PdfPage endPage, Point endPoint);
        PdfSelection? GetSelection(PdfPage startPage, int startCharIndex, PdfPage endPage, int endCharIndex);
        //void InsertPage(PdfPage page, int index);
        PdfOutline? OutlineItem(PdfSelection selection);
        void RemovePage(int index);
        //PdfSelection? SelectEntireDocument();
        //void SetDocumentAttributes(PdfDocumentAttributes? attributes);
        bool Unlock(string password);
        //bool Write(NSUrl url);
        //bool Write(NSUrl url, NSDictionary? options);
        //bool Write(NSUrl url, PdfDocumentWriteOptions options);
        //bool Write(string path);
        //bool Write(string path, NSDictionary? options);
        //bool Write(string path, PdfDocumentWriteOptions options);
    }
}