using ApplePDF.Extensions;
using ApplePDF.PdfKit.Annotation;
using CoreGraphics;
using Foundation;
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

        internal PdfPage(PdfDocument doc, int index)
        {
            Document = doc;
            PageIndex = index;
            page = Document?.Document.GetPage(PageIndex);
        }

        internal PdfPage(PdfDocument doc, iOSPdfKit.PdfPage platformPage)
        {
            Document = doc;
            page = platformPage;
            PageIndex = doc.GetPageIndex(this);
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
                    throw new ObjectDisposedException(nameof(Page));
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

        public int AnnotationCount => Page.Annotations.Length;

        public PdfAnnotation AddAnnotation(PdfAnnotationSubtype subtype)
        {
            PdfAnnotation pdfAnnotation = null;
            var annotation = new PlatformPdfAnnotation();
            switch (subtype)
            {
                case PdfAnnotationSubtype.Text:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Text)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfTextAnnotation(this, annotation, PdfAnnotationSubtype.Text);
                    break;
                case PdfAnnotationSubtype.Link:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Link)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfLinkAnnotation(this, annotation, PdfAnnotationSubtype.Link);
                    break;
                case PdfAnnotationSubtype.FreeText:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.FreeText)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfFreeTextAnnotation(this, annotation, PdfAnnotationSubtype.FreeText);
                    break;
                case PdfAnnotationSubtype.Line:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Line)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfLineAnnotation(this, annotation, PdfAnnotationSubtype.Line);
                    break;
                case PdfAnnotationSubtype.Square:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Square)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfSquareAnnotation(this, annotation, PdfAnnotationSubtype.Square);
                    break;
                case PdfAnnotationSubtype.Circle:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Circle)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfCircleAnnotation(this, annotation, PdfAnnotationSubtype.Circle);
                    break;
                case PdfAnnotationSubtype.Highlight:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Highlight)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfHighlightAnnotation(this, annotation, PdfAnnotationSubtype.Highlight);
                    break;
                case PdfAnnotationSubtype.Underline:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Underline)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfUnderlineAnnotation(this, annotation, PdfAnnotationSubtype.Underline);
                    break;
                case PdfAnnotationSubtype.Squiggly:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Squiggly)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfSquigglyAnnotation(this, annotation, PdfAnnotationSubtype.Squiggly);
                    break;
                case PdfAnnotationSubtype.StrikeOut:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.StrikeOut)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfStrikeoutAnnotation(this, annotation, PdfAnnotationSubtype.StrikeOut);
                    break;
                case PdfAnnotationSubtype.Ink:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Ink)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfInkAnnotation(this, annotation, PdfAnnotationSubtype.Ink);
                    break;
                case PdfAnnotationSubtype.Stamp:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Stamp)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfStampAnnotation(this, annotation, PdfAnnotationSubtype.Stamp);
                    break;
                case PdfAnnotationSubtype.Popup://Popup附着在其它注释上
                    throw new InvalidOperationException("Popup associated with other type annotation, it should be created form other annotation.");
                    break;
                case PdfAnnotationSubtype.Widget:
                    annotation.SetValue<NSString>(new NSString("/" + nameof(PdfAnnotationSubtype.Widget)), iOSPdfKit.PdfAnnotationKey.Subtype);
                    pdfAnnotation = new PdfWidgetAnnotation(this, annotation, PdfAnnotationSubtype.Widget);
                    break;
                case PdfAnnotationSubtype.Unknow:
                    pdfAnnotation = new PdfUnknowAnnotation(this, annotation, PdfAnnotationSubtype.Unknow);
                    break;
            }
            Page.AddAnnotation(annotation);
            return pdfAnnotation;
        }

        public PdfAnnotation[] GetAnnotations()
        {
            var platformAnnots = Page.Annotations;
            if (platformAnnots == null || platformAnnots.Length == 0)
                return null;
            var annotations = new List<PdfAnnotation>() { Capacity = platformAnnots.Length };
            for (int index = 0; index < platformAnnots.Length; index++)
            {
                var annotation = platformAnnots[index];
                var pdfAnnotation = GetAnnot(annotation);
                if (pdfAnnotation != null)
                    annotations.Add(pdfAnnotation);
            }
            return annotations.ToArray();
        }

        public PdfAnnotation? GetAnnotations(PointF point)
        {
            return GetAnnot(Page.GetAnnotation(point.ToCGPoint()));
        }

        private PdfAnnotation GetAnnot(PlatformPdfAnnotation annotation)
        {
            PdfAnnotation pdfAnnotation = null;
            var type = annotation.GetValue<NSString>(iOSPdfKit.PdfAnnotationKey.Subtype).ToString();
            switch (type)
            {
                case "/"+ nameof(PdfAnnotationSubtype.Text):
                    pdfAnnotation = new PdfTextAnnotation(this, annotation, PdfAnnotationSubtype.Text);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Link):
                    pdfAnnotation = new PdfLinkAnnotation(this, annotation, PdfAnnotationSubtype.Link);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.FreeText):
                    pdfAnnotation = new PdfFreeTextAnnotation(this, annotation, PdfAnnotationSubtype.FreeText);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Line):
                    pdfAnnotation = new PdfLineAnnotation(this, annotation, PdfAnnotationSubtype.Line);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Square):
                    pdfAnnotation = new PdfSquareAnnotation(this, annotation, PdfAnnotationSubtype.Square);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Circle):
                    pdfAnnotation = new PdfCircleAnnotation(this, annotation, PdfAnnotationSubtype.Circle);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Highlight):
                    pdfAnnotation = new PdfHighlightAnnotation(this, annotation, PdfAnnotationSubtype.Highlight);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Underline):
                    pdfAnnotation = new PdfUnderlineAnnotation(this, annotation, PdfAnnotationSubtype.Underline);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Squiggly):
                    pdfAnnotation = new PdfSquigglyAnnotation(this, annotation, PdfAnnotationSubtype.Squiggly);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.StrikeOut):
                    pdfAnnotation = new PdfStrikeoutAnnotation(this, annotation, PdfAnnotationSubtype.StrikeOut);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Ink):
                    pdfAnnotation = new PdfInkAnnotation(this, annotation, PdfAnnotationSubtype.Ink);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Stamp):
                    pdfAnnotation = new PdfStampAnnotation(this, annotation, PdfAnnotationSubtype.Stamp);
                    break;
                case "/" + nameof(PdfAnnotationSubtype.Popup)://Popup附着在其它注释上
                    break;               
                case "/" + nameof(PdfAnnotationSubtype.Widget):
                    pdfAnnotation = new PdfWidgetAnnotation(this, annotation, PdfAnnotationSubtype.Widget);
                    break;
                default:
                    pdfAnnotation = new PdfUnknowAnnotation(this, annotation, PdfAnnotationSubtype.Unknow);
                    break;
            }
            return pdfAnnotation;
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

        public PdfRectangleF[] GetCharactersBounds(int index, int count)
        {
            if (index < CharacterCount && (index + count) < CharacterCount)
            {
                if (count > 0)
                {
                    var result = new PdfRectangleF[count];
                    for (int rectIndex = 0; rectIndex < count; rectIndex++)
                        result[rectIndex] = Page.GetCharacterBounds(index + rectIndex).ToPdfRectangleF();
                    return result;    
                }
            }
            return null;
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
            page?.Dispose();
            page = null;
        }

        public SizeF GetSize()
        {
            var bounds = GetBoundsForBox(PdfDisplayBox.Media);
            return new SizeF(bounds.Width, bounds.Height);
        }

        public bool AddText(PdfFont font, float fontSize, Microsoft.Maui.Graphics.Text.IAttributedText text, double x, double y, double scale = 1)
        {
            int index = PageIndex;
            // 先绘制到一个新的文档
            var pdfData = new Foundation.NSMutableData();//动态数组
            var context = new CoreGraphics.CGContextPDF(new CGDataConsumer(pdfData));
            var page = this.Page;
            CoreGraphics.CGPDFPageInfo info = new CoreGraphics.CGPDFPageInfo();
            info.ArtBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Art);
            info.CropBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Crop);
            info.MediaBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);
            info.TrimBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Trim);
            context.BeginPage(info);//新页面
            context.InterpolationQuality = CGInterpolationQuality.High;
            // Draw existing page
            context.SaveState();
            //ctx.ScaleBy(x: 1, y: -1);
            //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
            context.DrawPDFPage(page.Page);
            var platformFont = new Microsoft.Maui.Graphics.Font(font.Font, Microsoft.Maui.Graphics.FontStyleType.Normal);
            var nsstring = Microsoft.Maui.Graphics.Platform.AttributedTextExtensions.AsNSAttributedString(text, platformFont);

            context.RestoreState();
            context.EndPage();

            context.Close();
            context.Dispose();
            using var newdoc = PdfKitLib.Instance.LoadPdfDocument(pdfData);
            //将新文档的新页面插入当前文档
            using var newdocpage = newdoc.GetPage(0);
            this.Document.Document.InsertPage(newdocpage.Page, this.PageIndex);
            var currentdocnewpage = this.Document.Document.GetPage(index);
            using var currentdocoldpage = this.Document.Document.GetPage(index + 1);
            this.page = currentdocnewpage;
            //将旧页面注释转移到新页面
            foreach (var annot in currentdocoldpage.Annotations)
            {
                currentdocnewpage.AddAnnotation(annot);
            }
            //移除旧页面
            this.Document.Document.RemovePage(index + 1);
            return true;
        }

        public bool AddText(PdfFont font, float fontSize, Color color, string text, double x, double y, double scale = 1)
        {
            int index = PageIndex;
            // 先绘制到一个新的文档
            var pdfData = new Foundation.NSMutableData();//动态数组
            var context = new CoreGraphics.CGContextPDF(new CGDataConsumer(pdfData));
            var page = this.Page;
            CoreGraphics.CGPDFPageInfo info = new CoreGraphics.CGPDFPageInfo();
            info.ArtBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Art);
            info.CropBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Crop);
            info.MediaBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);
            info.TrimBox = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Trim);
            context.BeginPage(info);//新页面
            context.InterpolationQuality = CGInterpolationQuality.High;
            // Draw existing page
            context.SaveState();
            //ctx.ScaleBy(x: 1, y: -1);
            //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
            context.DrawPDFPage(page.Page);

            context.SetFillColor(color.ToCGColor());
            context.SetFont(font.Font);
            context.SetFontSize(fontSize);
            context.SetTextDrawingMode(CGTextDrawingMode.Fill);
            context.ShowTextAtPoint((nfloat)x, (nfloat)y, text);

            context.RestoreState();
            context.EndPage();

            context.Close();
            context.Dispose();
            using var newdoc = PdfKitLib.Instance.LoadPdfDocument(pdfData);
            //将新文档的新页面插入当前文档
            using var newdocpage = newdoc.GetPage(0);
            this.Document.Document.InsertPage(newdocpage.Page, this.PageIndex);
            var currentdocnewpage = this.Document.Document.GetPage(index);
            using var currentdocoldpage = this.Document.Document.GetPage(index + 1);
            this.page = currentdocnewpage;
            //将旧页面注释转移到新页面
            foreach (var annot in currentdocoldpage.Annotations)
            {
                currentdocnewpage.AddAnnotation(annot);
            }
            //移除旧页面
            this.Document.Document.RemovePage(index + 1);
            return true;
        }

        public bool InsteadText(string oldText, string newText)
        {
            throw new NotImplementedException();
        }
    }
}