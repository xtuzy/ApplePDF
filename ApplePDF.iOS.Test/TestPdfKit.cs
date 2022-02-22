using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfKit;
using UIKit;
using CoreGraphics;
using Foundation;

namespace ApplePDF.iOS.Test
{
    [TestFixture]
    public class TestPdfKit
    {
        [Test]
        public void TestMethod()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
            PdfPage page;
            PdfDocument document;
            PdfAnnotation annotation;
            PdfAction action;
            PdfBorder border;
            PdfOutline outline;
            PdfSelection selection;
            PdfDisplayBox box;
            PdfAnnotationSubtype subtype;
            PdfAnnotationSubtypeExtensions extensions;
            PdfAnnotationHighlightingMode h;
            PdfAnnotationHighlightingModeExtensions h2;
            PdfAnnotationKey h3;
            PdfAnnotationKeyExtensions key;
            PdfAnnotationLineEndingStyle h2;
            PdfAnnotationLineEndingStyleExtensions h3;
            PdfAnnotationTextIconType textIconType;
            PdfAnnotationTextIconTypeExtensions textIconTypeExtensions;
            PdfAnnotationWidgetSubtype widgetSubtype;
            PdfAnnotationWidgetSubtypeExtensions pdfAnnotationWidgetSubtype;
            PdfTextAnnotationIconType annotationIconType;
            
        }

        

        //void DrawOnPage()
        //{
        //    var inUrl = new NSUrl("/Users/mayoff/Desktop/test.pdf");
        //    var outUrl = new NSUrl("/Users/mayoff/Desktop/testout.pdf");

        //    var doc = new PdfDocument(inUrl);
        //    var page = doc.GetPage(0);
        //    var mediaBox = page.GetBoundsForBox(PdfDisplayBox.Media);

        //    var gc = new CGContext(outUrl, mediaBox: &mediaBox, nil);
        //    var nsgc = new CGContextRef(cgContext: gc, flipped: false);
        //    NSGraphicsContext.current = nsgc;
        //    gc.beginPDFPage(null);
        //    do
        //    {
        //        page.Draw(PdfDisplayBox.Media, gc);

        //        var style = new NSMutableParagraphStyle();
        //        style.Alignment = UITextAlignment.Center;

        //        var richText = new NSAttributedString(
        //            "Hello, world!",
        //            new UIStringAttributes()
        //            {
        //                Font = UIFont.SystemFontOfSize(64),
        //                ForegroundColor = UIColor.Red,
        //                ParagraphStyle = NSParagraphStyle.Default
        //            }
        //            );

        //        var richTextBounds = richText.size();
        //        var point = CGPoint(x: mediaBox.midX - richTextBounds.width / 2, y: mediaBox.midY - richTextBounds.height / 2);
        //        gc.saveGState();
        //        do
        //        {
        //            gc.translateBy(x: point.x, y: point.y);
        //            gc.rotate(by: .pi / 5);
        //            richText.draw(at: .zero);
        //        }; gc.restoreGState();

        //    };
        //    gc.endPDFPage();
        //    NSGraphicsContext.current = nil;
        //    gc.closePDF();
        //}
    }
}
