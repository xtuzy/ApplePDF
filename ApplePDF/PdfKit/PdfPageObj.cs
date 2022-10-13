using PDFiumCore;
using System;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    /// <summary>
    /// <see cref="FpdfPageobjectT"/>
    /// </summary>
    public abstract class PdfPageObj : IDisposable
    {
        const string TAG = nameof(PdfPageObj);

        /// <summary>
        /// 默认为0, 新建的PageObj设置为1, 添加到Pdf后设置为2.用于<see cref="Dispose"/>内部.
        /// 原因:自己新建的<see cref="FpdfPageobjectT"/>如果没有添加到Pdf,需要调用<see cref="fpdf_edit.FPDFPageObjDestroy"/>释放内存,
        /// 而如果是添加到了Pdf的,或者是已存在的,调用Destroy方法会造成内存占用冲突.
        /// </summary>
        internal int PageObjTag = 0;
        public FpdfPageobjectT PageObj { get; private set; }

        public PdfPageObjectTypeFlag Type { get; private set; }

        public PdfPageObj(FpdfPageobjectT pageObj, PdfPageObjectTypeFlag type)
        {
            PageObj = pageObj;
            Type = type;
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        /// <param name="strokeColor"></param>
        public void SetStrokeColor(Color? strokeColor)
        {
            // text stroke color
            if (strokeColor != null)
            {
                if (fpdf_edit.FPDFPageObjSetStrokeColor(PageObj, strokeColor.Value.R, strokeColor.Value.G, strokeColor.Value.B, strokeColor.Value.A) == 0)
                {
                    Debug.WriteLine($"{TAG}:Fail to set stroke color to obj");
                }
            }
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        /// <param name="fillColor"></param>
        public void SetFillColor(Color? fillColor)
        {
            // text fill color
            if (fillColor != null)
            {
                if (fpdf_edit.FPDFPageObjSetFillColor(PageObj, fillColor.Value.R, fillColor.Value.G, fillColor.Value.B, fillColor.Value.A) == 0)
                {
                    Debug.WriteLine($"{TAG}:Fail to set fill color to obj");
                }
            }
        }

        public Color? GetFillColor()
        {
            // 颜色
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var success = fpdf_edit.FPDFPageObjGetFillColor(PageObj, ref R, ref G, ref B, ref A) == 1;
            if (success)
            {
                return System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            }
            else
            {
                Debug.WriteLine($"{TAG}:No fill color");
                return null;
            }
        }

        public Color? GetStrokeColor()
        {
            // 颜色
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var success = fpdf_edit.FPDFPageObjGetStrokeColor(PageObj, ref R, ref G, ref B, ref A) == 1;

            if (success)
            {
                return System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            }
            else
            {
                Debug.WriteLine($"{TAG}:No stroke color");
                return null;
            }
        }

        /// <summary>
        /// Obj要添加到Pdf, 最后必须调用此方法才最终添加了.
        /// </summary>
        public void Dispose() => Dispose(true);

        bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                if (PageObj != null && PageObjTag == 1)
                {
                    fpdf_edit.FPDFPageObjDestroy(PageObj);//看注释意思好像是,如果新建的没有被添加到Pdf就要调用释放资源,应该要设置一个tag标记是否添加到了页面
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            PageObj = null;
            _disposed = true;
        }
    }
}
