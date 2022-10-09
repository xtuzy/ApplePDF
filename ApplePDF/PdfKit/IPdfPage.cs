using PDFiumCore;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ApplePDF.PdfKit
{
    public interface IPdfPage
    {
        /// <summary>
        /// iOS:Gets an array that contains all the annotations on the PDF page.
        /// </summary>
        List<PdfAnnotation> Annotations { get; }
        //NSAttributedString? AttributedString { get; }
        /// <summary>
        /// iOS:Gets the number of characters in the text content of the PDF page.
        /// </summary>
        int CharacterCount { get; }
        //System.IntPtr ClassHandle { get; }
        //NSData? DataRepresentation { get; }
        /// <summary>
        /// iOS:Gets or sets a Boolean value that controls whether annotations are displayed.
        /// </summary>
        bool DisplaysAnnotations { get; set; }
        /// <summary>
        /// iOS:Gets the PDF document object that contains the PDF page.
        /// </summary>
        PdfDocument? Document { get;}
        //string Label { get; }
        /// <summary>
        /// iOS:Gets the Core Graphics PDFPage object for this PDF page
        /// </summary>
        FpdfPageT? Page { get; }
        /// <summary>
        /// iOS:Gets or sets the rotation, in degrees, for displaying the page.
        /// </summary>
        PdfRotate Rotation { get; set; }
        /// <summary>
        /// Gets the text content of the page.
        /// </summary>
        string Text { get; }
        /// <summary>
        /// iOS:Adds the specified annotation to the PDF page.
        /// </summary>
        /// <param name="annotation"></param>
        void AddAnnotation(PdfAnnotation annotation);
        //NSObject Copy(NSZone? zone);
        //void Draw(PdfDisplayBox box, CGContext context);
        /// <summary>
        /// iOS:Returns the annotation for the specified point on the page, or null if the point is not annotated.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        PdfAnnotation? GetAnnotations(PointF point);
        /// <summary>
        /// iOS:Returns a rectangle that describes the bounds for the character at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        PdfRectangleF GetCharacterBounds(int index);
        /// <summary>
        /// iOS:Returns the index of the character at the specified point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        int GetCharacterIndex(PointF point);
        /// <summary>
        /// iOS:Returns the text in the rectangle that is specified by the user-coordinate-space start and end points.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        PdfSelection? GetSelection(PointF startPoint, PointF endPoint);
        /// <summary>
        /// iOS:Returns the text in the specified rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        PdfSelection? GetSelection(PdfRectangleF rect);
        //PdfSelection? GetSelection(NSRange range);
        /// <summary>
        /// 获取缩略图，iOS默认返回UIImage
        /// </summary>
        /// <param name="size"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        object GetThumbnail(Size size, PdfDisplayBox box);
        //CGAffineTransform GetTransform(PdfDisplayBox box);
        /// <summary>
        /// iOS:Removes the specified annotation.
        /// </summary>
        /// <param name="annotation"></param>
        void RemoveAnnotation(PdfAnnotation annotation);
        /// <summary>
        /// iOS:Returns the line of text that is under the specified point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        PdfSelection? SelectLine(PointF point);
        /// <summary>
        /// iOS:Returns the word that is under the specified point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        PdfSelection? SelectWord(PointF point);
        PdfRectangleF GetBoundsForBox(PdfDisplayBox pdfDisplayBox);
        //void SetBoundsForBox(Rectangle bounds, PdfDisplayBox box);
        //void TransformContext(CGContext context, PdfDisplayBox box);
    }
}