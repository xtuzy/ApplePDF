using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// Annotation that use <see cref="PdfPageObj"/>.
    /// </summary>
    public interface IPdfPageObjAnnotation
    {
        /// <summary>
        /// If is array, that mean these <see cref="PdfPageObj"> can't change, this annot can't add <see cref="PdfPageObjs"/>.
        /// </summary>
        public List<PdfPageObj> PdfPageObjs { get; }
    }
}
