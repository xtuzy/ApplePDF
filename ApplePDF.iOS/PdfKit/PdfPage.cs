using ApplePDF.Extensions;
using CoreGraphics;
using CoreMedia;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public class PdfPage : IPdfPage
    {
        /// <summary>
        /// 从0开始
        /// <br/>#ApplePDF Api
        /// </summary>
        public int PageIndex { get; private set; }

        public PdfPage(PdfDocument doc, int index)
        {
            Document = doc;
            PageIndex = index;
        }

        PdfPage(PdfDocument doc, iOSPdfKit.PdfPage platformPage)
        {
            Document = doc;
            page = platformPage;
            PageIndex = doc.GetPageIndex(this);
        }

        public static PdfPage Create(PdfDocument doc, iOSPdfKit.PdfPage platformPage)
        {
            return new PdfPage(doc, platformPage);
        }

        public int CharacterCount => (int)Page.CharacterCount;

        public bool DisplaysAnnotations { get => Page.DisplaysAnnotations; set => Page.DisplaysAnnotations = value; }

        public PdfDocument? Document { get; private set; }

        PlatformPdfPage page;
        public PlatformPdfPage? Page
        {
            get
            {
                if (page == null)
                    page = Document?.Document.GetPage(PageIndex);
                return page;
            }
        }

        public PdfRotate Rotation
        {
            get
            {
                int rotation = 0;
                if (Page.Rotation < 0)
                {
                    var count = Math.Abs(Page.Rotation) / 360;//比360大多少倍
                    rotation = (int)(360 * (count + 1) + Page.Rotation);//换成正值
                }
                var c = rotation / 90;
                if (c == 0 || c % 4 == 0)//0,360,720
                    return PdfRotate.Degree0;
                else if (c % 3 == 0)//270,630
                    return PdfRotate.Degree270;
                else if (c % 2 == 0)//180,540
                    return PdfRotate.Degree180;
                else if (c % 1 == 0)//90,450
                    return PdfRotate.Degree90;
                else
                    return PdfRotate.Degree0;
            }
            set
            {
                switch (value)
                {
                    case PdfRotate.Degree0:
                        Page.Rotation = 0;
                        break;
                    case PdfRotate.Degree90:
                        Page.Rotation = 90;
                        break;
                    case PdfRotate.Degree180:
                        Page.Rotation = 180;
                        break;
                    case PdfRotate.Degree270:
                        Page.Rotation = 270;
                        break;
                    default:
                        Page.Rotation = 0;
                        break;
                }
            }
        }

        public string Text => Page.Text;

        public int AnnotationCount => throw new NotImplementedException();

        public void AddAnnotation(PdfAnnotation annotation)
        {
            Page.AddAnnotation(annotation.Annotation);
        }

        public System.Collections.Generic.List<PdfAnnotation> GetAnnotations()
        {
            List<PdfAnnotation> annotations = new List<PdfAnnotation>();
            foreach (var annotation in Page.Annotations)
            {
                annotations.Add(new PdfAnnotation(annotation));
            }
            return annotations;
        }

        public PdfAnnotation? GetAnnotations(PointF point)
        {
            return new PdfAnnotation(Page.GetAnnotation(point.ToCGPoint()));
        }

        public PdfRectangleF GetBoundsForBox(PdfDisplayBox pdfDisplayBox)
        {
            iOSPdfKit.PdfDisplayBox displayBox = (iOSPdfKit.PdfDisplayBox)pdfDisplayBox;
            var rect = Page.GetBoundsForBox(displayBox);
            return PdfRectangleF.FromLTRB((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        }

        public PdfRectangleF GetCharacterBounds(int index)
        {
            var rect = Page.GetCharacterBounds(index);
            return PdfRectangleFExtension.ToPdfRectangleF(rect);
        }

        public int GetCharacterIndex(PointF point)
        {
            return (int)Page.GetCharacterIndex(new CoreGraphics.CGPoint(point.X, point.Y));
        }

        public PdfSelection? GetSelection(PointF leftPoint, PointF rightPoint)
        {
            if (leftPoint.Y < rightPoint.Y)
                return new PdfSelection(this, PdfRectangleF.FromLTRB(leftPoint.X, rightPoint.Y, rightPoint.X, leftPoint.Y));
            return new PdfSelection(this, PdfRectangleF.FromLTRB(leftPoint.X, leftPoint.Y, rightPoint.X, rightPoint.Y));
        }

        public PdfSelection? GetSelection(PdfRectangleF rect)
        {
            return new PdfSelection(this, rect);
        }

        public object GetThumbnail(Size size, PdfDisplayBox box)
        {
            return Page.GetThumbnail(new CoreGraphics.CGSize(size.Width, size.Height), (iOSPdfKit.PdfDisplayBox)box);
        }

        public void RemoveAnnotation(PdfAnnotation annotation)
        {
            Page.RemoveAnnotation(annotation.Annotation);
        }

        public PdfSelection? SelectLine(PointF point)
        {
            return new PdfSelection(Page.SelectLine(point.ToCGPoint()));
        }

        public PdfSelection? SelectWord(PointF point)
        {
            return new PdfSelection(Page.SelectWord(point.ToCGPoint()));
        }

#if IOS || MACCATALYST
        /// <summary>
        /// 自身收集一个context？然后让page绘制，最后创建一个新的image？应该有内存复制
        /// </summary>
        /// <param name="xScale"></param>
        /// <returns></returns>
        public UIKit.UIImage Draw(float xScale)
        {
            UIKit.UIGraphics.BeginImageContextWithOptions(Page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media).Size, true, xScale);
            var context = UIKit.UIGraphics.GetCurrentContext();
            Page.Draw(iOSPdfKit.PdfDisplayBox.Media, context);
            var image = UIKit.UIGraphics.GetImageFromCurrentImageContext();
            UIKit.UIGraphics.EndImageContext();//TODO:不知道这个是存储到Image还是之后就不能绘图了
            return image;
            context.DrawPDFPage(Page.Page);//直接绘制CGPDFPage
        }

        /// <summary>
        /// 由已知的图像产生context，page绘制到这个context，然后产生新image，我不知道原来的image就只提供数据？
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <returns></returns>
        public UIKit.UIImage Draw(UIKit.UIImage sourceImage)
        {
            UIKit.UIGraphics.BeginImageContext(sourceImage.Size);
            sourceImage.Draw(new CoreGraphics.CGRect(0, 0, sourceImage.Size.Width, sourceImage.Size.Height));
            var context = UIKit.UIGraphics.GetCurrentContext();
            Page.Draw(iOSPdfKit.PdfDisplayBox.Media, context);
            var image = UIKit.UIGraphics.GetImageFromCurrentImageContext();
            UIKit.UIGraphics.EndImageContext();//TODO:不知道这个是存储到Image还是之后就不能绘图了
            return image;
        }
#endif
        /// <summary>
        /// 参考https://stackoverflow.com/a/41640262/13254773, 从数组指针创建context给page绘制.
        /// 它应该是直接操作数组，应该没有内存复制
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="w">图像宽</param>
        /// <param name="h"></param>
        public void Draw(IntPtr bufferPointer, int width, bool renderAnnot)
        {
            DisplaysAnnotations = renderAnnot;
            var context = new CGBitmapContext(bufferPointer, width, (nint)(width / GetSize().Width * GetSize().Height), 8, 4 * width, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.NoneSkipLast);
            Page.Draw(iOSPdfKit.PdfDisplayBox.Media, context);
            context.Flush();
        }

        /// <summary>
        /// 绘制到宽高为多少的图片上. 该方法内部创建字节数组提供给CGBitmapContext绘制图像, 但不知道<see cref="CGBitmapContext.ToImage"/>是否另占用内存.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="renderAnnot"></param>
        /// <returns></returns>
        public CGImage Draw(int w, int h, bool renderAnnot)
        {
            DisplaysAnnotations = renderAnnot;
            byte[] bufferPointer = new byte[w * h * 4];
            var context = new CGBitmapContext(bufferPointer, w, h, 8, 4 * w, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.NoneSkipLast);
            Page.Draw(iOSPdfKit.PdfDisplayBox.Media, context);
            context.Flush();
            return context.ToImage();
        }

        public void Dispose()
        {
            Document = null;
            Page?.Dispose();
            page = null;
        } 

        public SizeF GetSize()
        {
            var bounds = GetBoundsForBox(PdfDisplayBox.Media);
            return new SizeF(bounds.Width, bounds.Height);
        }

        public bool AddText(PdfFont font, float fontSize, string text, double x, double y, double scale = 1)
        {
            throw new NotImplementedException();
        }

        public bool InsteadText(string oldText, string newText)
        {
            throw new NotImplementedException();
        }
    }
}