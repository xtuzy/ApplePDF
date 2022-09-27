using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Demo.Maui.Services
{
    public struct OcrData
    {
        public List<OcrData> Childs;
        public string Text;
        public Microsoft.Maui.Graphics.Rect Bounds;
        /// <summary>
        /// 用于行文字大小和基线位置绘制
        /// </summary>
        public Microsoft.Maui.Graphics.Rect BaselineBounds;
    }
}
