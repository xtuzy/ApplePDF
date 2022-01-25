using CoreGraphics;
using Foundation;
using PdfKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Pdf.Net.iOS.Extension
{
    public static class PdfPageExtension
    {
        public static UIImage RenderToImage(PdfPage pdfpage, float width)
        {
            var page = pdfpage.Page;
            CGRect pageRect = page.GetBoxRect(CGPDFBox.Media); ;
            var pdfScale = width / pageRect.Size.Width;
            pageRect.Size = new CGSize(pageRect.Size.Width * pdfScale, pageRect.Size.Height * pdfScale);
            //pageRect.Origin = CGPointZero;

            UIGraphics.BeginImageContext(pageRect.Size);
            CGContext context = UIGraphics.GetCurrentContext();

            //White BG
            context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
            context.FillRect(pageRect);
            context.SaveState();

            // Next 3 lines makes the rotations so that the page look in the right direction
            context.TranslateCTM(0.0f, pageRect.Size.Height);
            context.ScaleCTM(1.0f, -1.0f);
            CGAffineTransform transform = page.GetDrawingTransform(CGPDFBox.Media, pageRect, 0, true);
            context.ConcatCTM(transform);

            context.DrawPDFPage(page);
            context.RestoreState();

            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return img;
        }
    }
}