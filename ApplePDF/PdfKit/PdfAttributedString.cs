using PDFiumCore;
using System;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfAttributedString : IDisposable
    {
        /// <summary>
        /// 获取除<see cref="Text"/>,<see cref="Bounds"/>,<see cref="StartIndex"/>外的属性需要先设置此Page，因为需要通过Page来获取这些属性
        /// </summary>
        public PdfPage Page { get; set; }

        public string Text { get; }
        /// <summary>
        /// 文本在Page中的Index
        /// </summary>
        public int StartIndex { get; }
        /// <summary>
        /// 文本所在的矩形边界
        /// </summary>
        public RectangleF Bounds { get; }

        public float Angle
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get Angle");

                var result = fpdf_text.FPDFTextGetCharAngle(Page.TextPage, StartIndex);
                if (result != -1)
                    return result;
                else
                    throw new NotImplementedException("获取FontWeight出错，暂未对其分析处理");
            }
        }

        public double FontSize
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get FontSize");
                return fpdf_text.FPDFTextGetFontSize(Page.TextPage, StartIndex);
            }
        }

        public double FontWeight
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get FontSize");
                var result = fpdf_text.FPDFTextGetFontWeight(Page.TextPage, StartIndex);
                if(result != -1)
                    return result;
                else
                    throw new NotImplementedException("获取FontWeight出错，暂未对其分析处理");
            }
        }

        public unsafe string FontName
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get FontSize");
                int flag = 0;
                byte[] buffer;
                //先填null获取字体名长度
                uint count = fpdf_text.FPDFTextGetFontInfo(Page.TextPage, StartIndex, IntPtr.Zero, 0, ref flag);
                if (count != 0)
                {
                    buffer = new byte[count];
                    fixed (byte* p = buffer)
                    {
                        IntPtr ptr = (IntPtr)p;
                        fpdf_text.FPDFTextGetFontInfo(Page.TextPage, StartIndex, ptr, count, ref flag);
                    }
                    //参考:https://stackoverflow.com/a/274207/13254773
                    string result = Encoding.UTF8.GetString(buffer);
                    return result;
                }
                else
                {
                    throw new NotImplementedException("获取FontName出错，暂未对其分析处理");
                }
            }
        }

        public Color StrokeColor
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get StrokeColor");
                uint r = default;
                uint g = default;
                uint b = default;
                uint a = default;
                if (fpdf_text.FPDFTextGetStrokeColor(Page.TextPage, StartIndex, ref r, ref g, ref b, ref a) == 0)
                {
                    return Color.Transparent;
                }
                else
                {
                    return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
                }
            }
        }

        public Color FillColor
        {
            get
            {
                if (Page == null)
                    throw new ArgumentException("You need set Page before get FillColor");
                uint r = default;
                uint g = default;
                uint b = default;
                uint a = default;
                if (fpdf_text.FPDFTextGetFillColor(Page.TextPage, StartIndex, ref r, ref g, ref b, ref a) == 0)
                {
                    return Color.Transparent;
                }
                else
                {
                    return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
                }
            }
        }

        public PdfAttributedString(string str, RectangleF box, int index)
        {
            Text = str;
            Bounds = box;
            StartIndex = index;
        }

        public void Dispose()
        {
            Page = null;
        }
    }
}
