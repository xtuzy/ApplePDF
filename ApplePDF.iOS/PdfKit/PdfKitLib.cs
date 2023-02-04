using CoreGraphics;
using System.IO;
#if IOS || MACCATALYST
using UIKit;
#endif
namespace ApplePDF.PdfKit
{
    public class PdfKitLib : ILib
    {
        public static PdfKitLib Instance = new PdfKitLib();
        PdfKitLib() { }

        public PdfDocument LoadPdfDocument(Stream stream, string password)
        {
            return new PdfDocument(stream, password);
        }

        public PdfDocument LoadPdfDocument(byte[] bytes, string password)
        {
            return new PdfDocument(bytes, password);
        }

        public PdfDocument LoadPdfDocument(string filePath, string password)
        {
            return new PdfDocument(filePath, password);
        }

        /// <summary>
        /// <see cref="Foundation.NSData"/>和byte[]类似, 提供相关的方法可以避免类型转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public PdfDocument LoadPdfDocument(Foundation.NSData data)
        {
            return new PdfDocument(new PlatformPdfDocument(data));
        }

        public PdfDocument CreatePdfDocument()
        {
            return new PdfDocument(new PlatformPdfDocument());
        }

        public void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream)
        {
            using var data = Merge(firstDoc, secondDoc);
            data.AsStream().CopyTo(stream);
        }

        public Foundation.NSMutableData Merge(PdfDocument firstDoc, PdfDocument secondDoc)
        {
            var pdfData = new Foundation.NSMutableData();//动态数组
#if MACOS || IOS || MACCATALYST
            var context = new CoreGraphics.CGContextPDF(new CGDataConsumer(pdfData));
            for (var index = 0; index < firstDoc.PageCount; index++)
            {
                using var page = firstDoc.Document.GetPage(index);
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
                context.RestoreState();
                context.EndPage();
            }

            for (var index = 0; index < secondDoc.PageCount; index++)
            {
                var page = secondDoc.Document.GetPage(index);
                var pageRect = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);

                context.BeginPage(pageRect);//新页面
                context.InterpolationQuality = CGInterpolationQuality.High;
                // Draw existing page
                context.SaveState();
                //ctx.ScaleBy(x: 1, y: -1);
                //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
                context.DrawPDFPage(page.Page);
                context.RestoreState();
                context.EndPage();
            }
            context.Close();
            context.Dispose();
#else //有bug
            // 参考https://gist.github.com/nyg/b8cd742250826cb1471f, 其从数组创建PdfContext, 然后绘制
            UIKit.UIGraphics.BeginPDFContext(pdfData, CGRect.Empty, null);
            for (var index = 0; index < firstDoc.PageCount; index++)
            {
                var page = firstDoc.Document.GetPage(index);
                var pageRect = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);

                UIGraphics.BeginPDFPage(pageRect, null);//新页面
                var ctx = UIGraphics.GetCurrentContext();//获取新页面的context
                ctx.InterpolationQuality = CGInterpolationQuality.High;
                // Draw existing page
                ctx.SaveState();
                //ctx.ScaleBy(x: 1, y: -1);
                //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
                ctx.DrawPDFPage(page.Page);
                ctx.RestoreState();
            }

            for (var index = 0; index < secondDoc.PageCount; index++)
            {
                var page = secondDoc.Document.GetPage(index);
                var pageRect = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);

                UIGraphics.BeginPDFPage(pageRect, null);
                var ctx = UIGraphics.GetCurrentContext();
                ctx.InterpolationQuality = CGInterpolationQuality.High;
                // Draw existing page
                ctx.SaveState();
                //ctx.ScaleBy(x: 1, y: -1);
                //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
                ctx.DrawPDFPage(page.Page);
                ctx.RestoreState();
            }

            //UIKit.UIGraphics.EndPDFContext();
#endif
            return pdfData;
        }

        public void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream)
        {
            Split(doc, fromePageIndex, toPageIndex).AsStream().CopyTo(stream);
        }

        public Foundation.NSData Split(PdfDocument doc, int fromePageIndex, int toPageIndex)
        {
#if MACOS || IOS || MACCATALYST
            var pdfData = new Foundation.NSMutableData();//动态数组
            var context = new CoreGraphics.CGContextPDF(new CGDataConsumer(pdfData));
            for (var index = fromePageIndex; index <= toPageIndex; index++)
            {
                var page = doc.Document.GetPage(index);
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
                context.RestoreState();
                context.EndPage();
            }
            context.Close();
            context.Dispose();
            return pdfData;
#else //uikit方法需要ui线程
            // iOS参考https://www.jianshu.com/p/b51a2f3067cb使用Render
            var render = new UIGraphicsPdfRenderer();
            var data = render.CreatePdf((context) =>
            {
                for (; fromePageIndex < toPageIndex; fromePageIndex++)
                {
                    context.BeginPage();
                    context.CGContext.DrawPDFPage(doc.GetPage(fromePageIndex).Page.Page);
                }
            });
            return data;
#endif
        }

        public bool Save(PdfDocument doc, string filePath)
        {
            return doc.Document.Write(filePath);
        }

        /// <summary>
        /// 使用了<see cref="iOSPdfKit.PdfDocument.GetDataRepresentation"/>, 貌似很慢, 不适用于大Pdf.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Save(PdfDocument doc, Stream stream)
        {
            var data = doc.Document.GetDataRepresentation();
            data.AsStream().CopyTo(stream);
            return true;
        }

        public byte[] Save(PdfDocument doc)
        {
            var data = doc.Document.GetDataRepresentation();
            return data.ToArray();
        }

        public void Dispose()
        {
        }
    }
}
