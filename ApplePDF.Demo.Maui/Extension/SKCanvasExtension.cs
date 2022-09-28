using SkiaSharp;

namespace ApplePDF.Demo.Maui.Extension
{
    public static class SKCanvasExtension
    {
        /// <summary>
        /// 在垂直的距离中心绘制水平文本
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="rect"></param>
        public static void DrawTextAtVerticalCenter(this SKCanvas canvas, string text, float x, float top, float bottom, SKPaint paint)
        {
            float y;
            y = top + (bottom - top) / 2 + GetBaseline(paint);
            canvas.DrawText(text, x, y, paint);
        }

        /// <summary>
        /// 在矩形中心绘制文本
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="rect"></param>
        public static void DrawTextAtCenter(this SKCanvas canvas, string text, SKRect rect, SKPaint paint)
        {
            float x, y;
            x = rect.Left + (rect.Width - paint.MeasureText(text)) / 2;
            y = rect.Top + (rect.Bottom - rect.Top) / 2 + GetBaseline(paint);
            canvas.DrawText(text, x, y, paint);
        }

        /// <summary>
        /// 计算绘制文字时的基线到中轴线的距离
        /// 参考:
        /// <see href="https://www.jianshu.com/p/057ce6b81c52">1</see>
        /// <see href="https://stackoverflow.com/questions/27631736/meaning-of-top-ascent-baseline-descent-bottom-and-leading-in-androids-font">2</see>
        /// <see href="https://liajoy.github.io/2018/10/26/canvas-text/">3</see>
        /// <see href="https://stackoverflow.com/questions/11120392/android-center-text-on-canvas">4</see>
        /// </summary>
        /// <param name="p"></param>
        /// <returns>基线和centerY的距离</returns>
        public static float GetBaseline(SKPaint p)
        {
            SKFontMetrics fontMetrics = p.FontMetrics;
            return (fontMetrics.Descent - fontMetrics.Ascent) / 2 - fontMetrics.Descent;
        }
    }
}
