using Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public class PdfAttributedString : IPdfAttributedString
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
        public PdfRectangleF Bounds { get; }

        public float Angle
        {
            get;
        }

        public double FontSize
        {
            get;
        }

        public double FontWeight
        {
            get;
        }

        public string FontName
        {
            get;
        }

        public Color StrokeColor
        {
            get;
        }

        public Color FillColor
        {
            get;
        }

        public PdfAttributedString(string str, PdfRectangleF box, int index)
        {
            Text = str;
            Bounds = box;
            StartIndex = index;
        }

        public void Dispose()
        {
            Page = null;
        }

        /// <summary>
        /// 分析iOS的NSAttributedString
        /// </summary>
        /// <param name="attributedString"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static List<PdfAttributedString> Phrase(NSAttributedString? attributedString)
        {
            throw new NotImplementedException();
        }
    }
}
