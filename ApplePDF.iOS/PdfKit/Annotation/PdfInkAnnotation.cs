using ApplePDF.Extensions;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using System.Drawing;
#if IOS || MACCATALYST
using PlatformPath = UIKit.UIBezierPath;
#else
using PlatformPath = AppKit.NSBezierPath;
#endif
namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// pdf有inklist key存储点，但在ios中通过inklist key和path获得的结果都是path，因此iOS上请直接使用path，pdfium上
    /// 先尝试获取<see cref="PdfPagePathObj"/>，如果没有再获取inkpoints
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation
    {
        internal PdfInkAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type)
            : base(page, annotation, type)
        {
        }

        public Color StrokeColor { get => Annotation.Color.ToColor(); set => Annotation.Color = value.ToUIColor(); }
        public Color? FillColor { get => Annotation.InteriorColor.ToColor(); set => Annotation.InteriorColor = value.Value.ToUIColor(); }

        public PlatformPath[] InkListPaths { get => Annotation.Paths; }
        
        public void AddPath(PlatformPath path)
        {
            Annotation.AddBezierPath(path);
        }

        public void RemovePath(PlatformPath path)
        {
            Annotation.RemoveBezierPath(path);
        }
    }
}
