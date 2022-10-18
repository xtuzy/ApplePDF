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
        public IEnumerable<PdfPageObj> PdfPageObjs { get; }

        /// <summary>
        /// 注释实际Obj数量,不是<see cref="PdfPageObjs"/>的数量, 主要用于在添加新的Obj时获取Index.
        /// </summary>
        /// <returns></returns>
        public int GetObjCount();
    }
}
