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

        public PdfDocument LoadPdfDocument(string filePath, string password)
        {
            return new PdfDocument(filePath, password);
        }
#if MACOS
        public void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream)
        {
            var pdfData = new Foundation.NSMutableData();//动态数组
            var context = new CoreGraphics.CGContextPDF(new CGDataConsumer(pdfData));
            for (var index = 0; index < firstDoc.PageCount; index++)
            {
                var page = firstDoc.Document.GetPage(index);
                var pageRect = page.GetBoundsForBox(iOSPdfKit.PdfDisplayBox.Media);

                context.BeginPage(pageRect);//新页面
                context.InterpolationQuality = CGInterpolationQuality.High;
                // Draw existing page
                context.SaveState();
                //ctx.ScaleBy(x: 1, y: -1);
                //ctx.TranslateBy(x: 0, y: -(pageRect?.size.height)!);
                context.DrawPDFPage(page.Page);
                context.RestoreState();
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
            }
            context.Dispose();
            pdfData.AsStream().CopyTo(stream);
        }


        public void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream)
        {
            throw new System.NotImplementedException();
        }
#endif

#if IOS || MACCATALYST
        /// <summary>
        /// 参考https://gist.github.com/nyg/b8cd742250826cb1471f, 其从数组创建PdfContext, 然后绘制
        /// </summary>
        /// <param name="firstDoc"></param>
        /// <param name="secondDoc"></param>
        /// <param name="stream"></param>
        public void Merge(PdfDocument firstDoc, PdfDocument secondDoc, Stream stream)
        {
            var pdfData = new Foundation.NSMutableData();//动态数组
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
            pdfData.AsStream().CopyTo(stream);
        }

        /// <summary>
        /// 参考https://www.jianshu.com/p/b51a2f3067cb使用Render
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fromePageIndex"></param>
        /// <param name="toPageIndex"></param>
        /// <param name="stream"></param>
        public void Split(PdfDocument doc, int fromePageIndex, int toPageIndex, Stream stream)
        {
            var render = new UIGraphicsPdfRenderer();
            var data = render.CreatePdf((context) =>
            {
                for (; fromePageIndex < toPageIndex; fromePageIndex++)
                {
                    context.BeginPage();
                    context.CGContext.DrawPDFPage(doc.GetPage(fromePageIndex).Page.Page);
                }
            });
            data.AsStream().CopyTo((stream));
        }
#endif
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
    }
}
