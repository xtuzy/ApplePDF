using PDFiumCore;
using System.Collections.Generic;
using System.Drawing;

namespace Pdf.Net.PdfKit
{
    public interface IPdfPage
    {
        List<PdfAnnotation> Annotations { get; }
        //NSAttributedString? AttributedString { get; }
        int CharacterCount { get; }
        //System.IntPtr ClassHandle { get; }
        //NSData? DataRepresentation { get; }
        bool DisplaysAnnotations { get; set; }
        //PdfDocument? Document { get;}
        //string Label { get; }
        FpdfPageT? Page { get; }
        PdfRotate Rotation { get; set; }
        string Text { get; }
        
        void AddAnnotation(PdfAnnotation annotation);
        //NSObject Copy(NSZone? zone);
        //void Draw(PdfDisplayBox box, CGContext context);
        PdfAnnotation? GetAnnotations(Point point);
        RectangleF GetBoundsForBox(PdfDisplayBox box);
        RectangleF GetCharacterBounds(int index);
        int GetCharacterIndex(Point point);
        PdfSelection? GetSelection(Point startPoint, Point endPoint);
        PdfSelection? GetSelection(Rectangle rect);
        //PdfSelection? GetSelection(NSRange range);
        //UIImage GetThumbnail(Size size, PdfDisplayBox box);
        //CGAffineTransform GetTransform(PdfDisplayBox box);
        void RemoveAnnotation(PdfAnnotation annotation);
        PdfSelection? SelectLine(Point point);
        PdfSelection? SelectWord(Point point);
        //void SetBoundsForBox(Rectangle bounds, PdfDisplayBox box);
        //void TransformContext(CGContext context, PdfDisplayBox box);
    }
}