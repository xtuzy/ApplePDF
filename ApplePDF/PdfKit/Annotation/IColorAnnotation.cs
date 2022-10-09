using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.PdfKit.Annotation
{
    public interface IColorAnnotation
    {

    }

    public interface IFillColorAnnotation: IColorAnnotation
    {
        /// <summary>
        /// FreeText的是FillColor
        /// </summary>
        public Color? FillColor { get;}
    }

    public interface IStrokeColorAnnotation : IColorAnnotation
    {
        public Color? StrokeColor { get; }
    }

    public interface IDefaultColorAnnotation : IColorAnnotation
    {
        public Color? AnnotColor { get; }
    }
}
