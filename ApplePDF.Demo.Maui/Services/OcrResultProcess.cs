
using MathNet.Numerics.Statistics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Demo.Maui.Services
{
    internal class OcrResultProcess
    {
        /// <summary>
        /// 绘制行矩形区域和基线
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="ocrLines"></param>
        public static void DrawLineRect(SKBitmap bitmap, List<OcrData> ocrLines)
        {
            using (var canvas = new SKCanvas(bitmap))
            {
                using (var paint = new SKPaint() { Color = SKColors.Red, Style = SKPaintStyle.Stroke })
                {
                    //绘制文字轮廓背景,用来看文字区域是否识别正确
                    paint.Color = SKColors.Green.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawRect((float)line.Bounds.X, (float)line.Bounds.Y, (float)line.Bounds.Width, (float)line.Bounds.Height, paint);
                    }
                    //绘制基线局域,用来看文字区域是否识别正确
                    paint.Color = SKColors.Red.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawLine((float)line.BaselineBounds.Left, (float)line.BaselineBounds.Top, (float)line.BaselineBounds.Right, (float)line.BaselineBounds.Bottom, paint);
                    }
                }
            }
        }

        /// <summary>
        /// 直接绘制一行
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="ocrLines"></param>
        public static void DrawLineText(SKBitmap bitmap, List<OcrData> ocrLines)
        {
            using (var canvas = new SKCanvas(bitmap))
            {
                using (var paint = new SKPaint() { Color = SKColors.Black, Typeface = SKFontManager.Default.MatchCharacter('中') })
                {
                    //绘制文字
                    paint.Color = SKColors.AliceBlue;
                    var linesTextSize = AnalysisTextSize(ocrLines);
                    foreach (var line in ocrLines)
                    {
                        /*
                         * 文字大小:行Bounds顶部减去行基线顶部
                         */
                        paint.TextSize = (float)line.BaselineBounds.Top - (float)line.Bounds.Top;
                        /*
                         * Y坐标:使用行基线
                         */

                        /*
                         * X坐标:
                         * 正常: 文本测量的宽度差不多
                         * 1. 行Bounds宽太大的,一般是少量文本,文本在行Bounds中间. 先测量行文本大小, 看是否行两端相差多个文字的间隔
                         * 2. 一行中间有空白的, 可以使用Word的定位. 先测量行文本大小是否差行宽多个文字间隔, 看中间是否有连续空格, 连续空格结束就是文字开始
                         */
                        var text = line.Text;
                        if (text == null || text == "")
                            continue;
                        var textWidth = paint.MeasureText(text.AsSpan());
                        canvas.DrawText(text, (float)(line.Bounds.Left), (float)line.BaselineBounds.Bottom, paint);
                    }
                }
            }
        }

        /// <summary>
        /// 通过Word位置纠正一行中部分文字位置（即一行可能拆分绘制）
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="ocrLines"></param>
        /// <exception cref="Exception"></exception>
        public static void DrawLineTextWithFixPosition(SKBitmap bitmap, List<OcrData> ocrLines)
        {
            using (var canvas = new SKCanvas(bitmap))
            {
                using (var paint = new SKPaint() { Color = SKColors.Black, Typeface = SKFontManager.Default.MatchCharacter('中') })
                {
                    //绘制文字
                    paint.Color = SKColors.AliceBlue;
                    var linesTextSize = AnalysisTextSize(ocrLines);
                    foreach (var line in ocrLines)
                    {
                        /*
                         * 文字大小:行Bounds顶部减去行基线顶部
                         */
                        paint.TextSize = (float)line.BaselineBounds.Top - (float)line.Bounds.Top;
                        /*
                         * Y坐标:使用行基线
                         */

                        /*
                         * X坐标:
                         * 正常: 文本测量的宽度差不多
                         * 1. 行Bounds宽太大的,一般是少量文本,文本在行Bounds中间. 先测量行文本大小, 看是否行两端相差多个文字的间隔
                         * 2. 一行中间有空白的, 可以使用Word的定位. 先测量行文本大小是否差行宽多个文字间隔, 看中间是否有连续空格, 连续空格结束就是文字开始
                         */
                        var text = line.Text;
                        var textWidth = paint.MeasureText(text.AsSpan());

                        if (textWidth + 6 * paint.TextSize < line.Bounds.Width)//测量的宽度偏小的
                        {
                            if (text.Length < 7)//如果是1.
                            {
                                canvas.DrawText(text, (float)(line.Bounds.Left + line.Bounds.Width / 2), (float)line.BaselineBounds.Bottom, paint);
                                continue;
                            }
                            else
                            {
                                //分割行成块
                                List<string> lineTextBlocks = new List<string>();
                                StringBuilder currentStr = new StringBuilder();
                                for (int i = 0; i < text.Length; i++)
                                {
                                    if (text[i] != ' ')
                                    {
                                        currentStr.Append(text[i]);
                                    }
                                    else//为空格
                                    {
                                        if (i + 1 < text.Length - 1 && text[i + 1] == ' ' && i + 2 < text.Length - 1 && text[i + 2] == ' ')//如果连续三字符都是空格,那么新建一个文本块
                                        {
                                            if (currentStr.Length > 0)//如果之前有字符,就更新一段
                                            {
                                                lineTextBlocks.Add(currentStr.ToString());
                                                currentStr.Clear();
                                            }
                                            else//没有就继续下一个字符
                                            {
                                                continue;
                                            }
                                        }
                                        //非连续空格时,如果之前的没有字符,那么跳过
                                        if (currentStr.Length > 0)
                                            currentStr.Append(text[i]);
                                        else
                                            continue;
                                    }
                                }

                                if (lineTextBlocks.Count > 0)//如果是2.真的有间隔
                                {
                                    //查找块的第一个Word
                                    int lastMatchIndex = 0;
                                    int lastMatchIndexEnd = 0;
                                    foreach (var textBlock in lineTextBlocks)//水平上间隔开的文本
                                    {
                                        bool isMatch = false;

                                        for (int i = lastMatchIndexEnd; i < line.Childs.Count; i++)
                                        {
                                            if (textBlock.Contains(line.Childs[i].Text))//如包含word
                                            {
                                                isMatch = true;
                                                lastMatchIndex = i;
                                                lastMatchIndexEnd = i + 1;
                                                break;
                                            }
                                        }

                                        if (isMatch)//匹配的话取第一个字符的位置作为开始位置
                                        {
                                            canvas.DrawText(textBlock, (float)(line.Childs[lastMatchIndex].Bounds.Left), (float)line.BaselineBounds.Bottom, paint);
                                        }
                                        else
                                        {
                                            throw new Exception("有文本没有匹配, 算法有问题");
                                        }
                                    }
                                }
                                else//没有间隔,只是measure的宽度偏小,正常绘制
                                {
                                    canvas.DrawText(text, (float)(line.Bounds.Left), (float)line.BaselineBounds.Bottom, paint);
                                }
                            }
                        }
                        else//测量的宽度差不多的,正常绘制
                        {
                            canvas.DrawText(text, (float)(line.Bounds.Left), (float)line.BaselineBounds.Bottom, paint);
                        }
                    }
                }
            }
        }

        public static Dictionary<Services.OcrData, float> AnalysisTextSize(List<Services.OcrData> lines)
        {
            var result = new Dictionary<Services.OcrData, float>();
            var heights = lines.Select(line => line.Bounds.Height).ToArray();
            var standardDeviation = heights.StandardDeviation();
            var mean = heights.Mean();
            var anomalyCutOff = standardDeviation * 3;
            var lowerLimit = mean - anomalyCutOff;
            var upperLimit = mean + anomalyCutOff;
            foreach (var line in lines)
            {
                if (line.Bounds.Height > lowerLimit && line.Bounds.Height < upperLimit)
                {
                    result.Add(line, (float)mean);
                }
                else
                {
                    result.Add(line, (float)line.Bounds.Height);
                }
            }
            return result;
        }
    }
}
