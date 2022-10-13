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
        public Color? AnnotColor { get; }
    }
}
