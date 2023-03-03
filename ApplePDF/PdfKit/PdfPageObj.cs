using PDFiumCore;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace ApplePDF.PdfKit
{
    /// <summary>
    /// <see cref="FpdfPageobjectT"/>
    /// </summary>
    public abstract class PdfPageObj : IDisposable
    {
        const string TAG = nameof(PdfPageObj);

        public enum TypeFlag
        {
            Unknow = PdfPageObjectTypeFlag.UNKNOWN,
            Text = PdfPageObjectTypeFlag.TEXT,
            Path = PdfPageObjectTypeFlag.PATH,
            Image = PdfPageObjectTypeFlag.IMAGE,
            Shading = PdfPageObjectTypeFlag.SHADING,
            Form = PdfPageObjectTypeFlag.FORM,
        }

        /// <summary>
        /// 默认为0, 新建的PageObj设置为1, 添加到Pdf后设置为2.用于<see cref="Dispose"/>内部.
        /// 原因:自己新建的<see cref="FpdfPageobjectT"/>如果没有添加到Pdf,需要调用<see cref="fpdf_edit.FPDFPageObjDestroy"/>释放内存,
        /// 而如果是添加到了Pdf的,或者是已存在的,调用Destroy方法会造成内存占用冲突.
        /// </summary>
        internal int PageObjTag = 0;
        /// <summary>
        /// Pdfium中获取Obj是按照Index获取的,从0开始
        /// </summary>
        internal int Index = -1;
        public FpdfPageobjectT PageObj { get; private set; }

        public TypeFlag Type { get; private set; }

        public PdfPageObj(FpdfPageobjectT pageObj, TypeFlag type)
        {
            PageObj = pageObj;
            Type = type;
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        public Color? FillColor
        {
            get
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

            set
            {
                // text fill color
                if (value.HasValue)
                {
                    if (fpdf_edit.FPDFPageObjSetFillColor(PageObj, value.Value.R, value.Value.G, value.Value.B, value.Value.A) == 0)
                    {
                        Debug.WriteLine($"{TAG}:Fail to set fill color to obj");
                    }
                }
            }
        }

        /// <summary>
        /// Support ink and stamp. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        public Color? StrokeColor
        {
            get
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

            set
            {
                if (value.HasValue)
                {
                    if (fpdf_edit.FPDFPageObjSetStrokeColor(PageObj, value.Value.R, value.Value.G, value.Value.B, value.Value.A) == 0)
                    {
                        Debug.WriteLine($"{TAG}:Fail to set stroke color to obj");
                    }
                }
            }
        }

        /// <summary>
        /// <para>|a c e|</para>
        /// <para>|b d f|</para>
        /// </summary>
        public void SetTranform(double a, double b, double c, double d, double e, double f)
        {
            fpdf_edit.FPDFPageObjTransform(PageObj, a, b, c, d, e, f);
        }

        public Matrix3x2? Matrix
        {
            get
            {
                // |          | a b 0 |
                // | matrix = | c d 0 |
                // |          | e f 1 |
                var matrix = new FS_MATRIX_();

                if (fpdf_edit.FPDFPageObjGetMatrix(PageObj, matrix) == 1)
                    return new Matrix3x2(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
                else
                    return null;
            }

            set
            {
                if (!value.HasValue) return;
                var matrix = new FS_MATRIX_();
                matrix.A = value.Value.M11;
                matrix.B = value.Value.M12;
                matrix.C = value.Value.M21;

                matrix.D = value.Value.M22;
                matrix.E = value.Value.M31;
                matrix.F = value.Value.M32;
                fpdf_edit.FPDFPageObjSetMatrix(PageObj, matrix);
            }
        }

        /// <summary>
        /// 获取/BBox的值, 见PDF32000_2008(8.10.2 Form Dictionaries)
        /// </summary>
        public PdfRectangleF Bounds
        {
            get
            {
                float l = 0;
                float t = 0;
                float r = 0;
                float b = 0;
                fpdf_edit.FPDFPageObjGetBounds(PageObj, ref l, ref b, ref r, ref t);
                return PdfRectangleF.FromLTRB(l, t, r, b);
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
