using ApplePDF.Extensions;
using System.Drawing;
#if IOS || MACCATALYST
using PlatformPath = UIKit.UIBezierPath;
#else
using PlatformPath = AppKit.NSBezierPath;
#endif
namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Ink的生成有两种,一种使用<see cref="InkListPaths"/>和<see cref="AnnotColor"/>,
    /// 另一种使用<see cref="PdfPagePathObj"/>, 建议使用后者
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation
    {
        internal PdfInkAnnotation(PdfPage page, PlatformPdfAnnotation annotation, PdfAnnotationSubtype type)
            : base(page, annotation, type)
        {
        }

        public Color StrokeColor { get => Annotation.Color.ToColor(); set => Annotation.Color = value.ToUIColor(); }

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
