#if WINDOWS
using ApplePDF.Demo.Maui.Extension;
using MathNet.Numerics.Statistics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using Windows.Globalization;

namespace ApplePDF.Demo.Maui.Services
{
    internal class TesseractOcrService
    {
        static TesseractEngine tesseractEngine;
        static string? modelFolderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "tessdata");
        static async Task InitTesseractDataAsync()
        {
            if (!Directory.Exists(modelFolderPath))
            {
                Directory.CreateDirectory(modelFolderPath);
            }
            //添加默认的模型
            if (!(new DirectoryInfo(modelFolderPath).GetFileSystemInfos().Length > 0))
            {
                string[] modelDatas = new[]
                {
                    @"chi_sim.traineddata",
                    @"eng.traineddata",
                    @"chi_sim_vert.traineddata",
                    @"chi_tra.traineddata"
                };
                //存储Tesseract的模型数据到文件夹
                foreach (var modelData in modelDatas)
                {
                    try
                    {
                        using (var stream = FileSystem.OpenAppPackageFileAsync("tessdata\\" + modelData))
                        {
                            var path = Path.Combine(modelFolderPath, modelData);
                            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                (await stream).CopyTo(fileStream);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        private readonly IMediaPicker mediaPicker;
        private ActivityIndicator activityIndicator;

        public TesseractOcrService(ActivityIndicator activityIndicator)
        {
            this.activityIndicator = activityIndicator;
            // Create Tesseract instance
            // InitTesseractDataAsync();
        }

        public async Task<string> RecogniseAppPhotoClickAsync(string lang = "eng")
        {
            string photoFileName = null;
            if (lang == "eng")
                photoFileName = @"photos\english.png";
            else
                photoFileName = @"photos\chinese.png";

            // save the file into local storage
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, photoFileName);

            using Stream sourceStream = await FileSystem.OpenAppPackageFileAsync(photoFileName);
            using var memoryStream = new MemoryStream();

            await sourceStream.CopyToAsync(memoryStream);
            return await Recognise(memoryStream, lang);
        }

        public async Task<string> RecogniseCapturePhotoClickAsync(string lang = "eng")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    using Stream sourceStream = await photo.OpenReadAsync();
                    using var memoryStream = new MemoryStream();

                    await sourceStream.CopyToAsync(memoryStream);
                    return await Recognise(memoryStream, lang);
                }
            }
            return null;
        }

        /// <summary>
        /// Recognizes the text by Tess.比windows自带的慢
        /// </summary>
        /// <param name="stream">must The memerystream.</param>
        /// <param name="modelName">chi_sim,chi_sim_vert,chi_tra</param>
        /// <returns></returns>
        public async Task<string> Recognise(MemoryStream stream, string lang = "chi_sim")
        {
            string text = null;
            try
            {
                activityIndicator.IsRunning = true;
                await Task.Run(() =>
                {
                    if (Directory.Exists(modelFolderPath))
                    {
                        if (!(new DirectoryInfo(modelFolderPath).GetFileSystemInfos().Length > 0))
                        {
                            InitTesseractDataAsync();
                        }
                    }
                    else
                        InitTesseractDataAsync();
                    if (tesseractEngine == null)
                        tesseractEngine = new TesseractEngine(modelFolderPath, lang, EngineMode.Default);
                    try
                    {
                        using (var img = Pix.LoadFromMemory(stream.ToArray())) //LoadFromFile(testImagePath))
                        {
                            using (var page = tesseractEngine.Process(img))
                            {
                                text = page.GetText();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                });
            }
            catch (System.Exception ex)
            {
                App.Current?.MainPage?.DisplayAlert("Error", ex.Message, "Cancel");
            }
            finally
            {
                activityIndicator.IsRunning = false;
            }
            Debug.WriteLine(text);
            return text;
        }

        /// <summary>
        /// 只一遍识别，对字符处理过的，返回数据包括行和Word的数据
        /// </summary>
        /// <param name="stream">must The memerystream.</param>
        /// <param name="modelName">chi_sim,chi_sim_vert,chi_tra</param>
        /// <returns></returns>
        public async Task<List<OcrData>> RecognizeWords(MemoryStream stream, string lang = "chi_sim")
        {
            List<OcrData> lines = new List<OcrData>();
            try
            {
                activityIndicator.Dispatcher.Dispatch(() => activityIndicator.IsRunning = true);
                await Task.Run(() =>
                {
                    if (Directory.Exists(modelFolderPath))
                    {
                        if (!(new DirectoryInfo(modelFolderPath).GetFileSystemInfos().Length > 0))
                        {
                            InitTesseractDataAsync();
                        }
                    }
                    else
                        InitTesseractDataAsync();
                    if (tesseractEngine == null)
                        tesseractEngine = new TesseractEngine(modelFolderPath, lang, EngineMode.Default);
                    try
                    {
                        using (var img = Pix.LoadFromMemory(stream.ToArray())) //LoadFromFile(testImagePath))
                        {
                            using (var page = tesseractEngine.Process(img))
                            {
                                Debug.WriteLine(page.GetText());
                                //Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                                using (var iter = page.GetIterator())
                                {
                                    iter.Begin();
                                    var language = iter.GetWordRecognitionLanguage();
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                //行
                                                var lineLevel = PageIteratorLevel.TextLine;
                                                iter.TryGetBoundingBox(lineLevel, out var lineRect);
                                                iter.TryGetBaseline(lineLevel, out var lineBaselineRect);
                                                var lineText = iter.GetText(lineLevel).Replace('\n', ' ');
                                                //判断是否单含有中文(不判断字符),如果含有中文,则纠正英文字符为中文字符,如果没有中文,则纠正中文字符为英文字符.
                                                //参考:https://blog.51cto.com/u_3664660/3213618
                                                var existChinese = lineText.Matches("[\u4e00-\u9fa5]");
                                                if (existChinese)
                                                {
                                                    //替换英文字符
                                                    lineText = EnglishPunctuation2Chineseunctuation(lineText);
                                                }
                                                else
                                                {
                                                    //替换中文字符
                                                    lineText = ChinesePunctuation2EnglishPunctuation(lineText);
                                                }
                                                var line = new OcrData()
                                                {
                                                    Childs = new List<OcrData>(),
                                                    Text = lineText,
                                                    Bounds = new Microsoft.Maui.Graphics.Rect(lineRect.X1, lineRect.Y1, lineRect.Width, lineRect.Height),
                                                    BaselineBounds = new Microsoft.Maui.Graphics.Rect(lineBaselineRect.X1, lineBaselineRect.Y1, lineBaselineRect.Width, lineBaselineRect.Height),
                                                };
                                                lines.Add(line);
                                                do
                                                {
                                                    //Word
                                                    var level = PageIteratorLevel.Word;
                                                    iter.TryGetBoundingBox(level, out var rect);
                                                    iter.TryGetBaseline(level, out var baselineRect);
                                                    line.Childs.Add(new OcrData()
                                                    {
                                                        Text = iter.GetText(level).Replace('\n', ' '),
                                                        Bounds = new Microsoft.Maui.Graphics.Rect(rect.X1, rect.Y1, rect.Width, rect.Height),
                                                        BaselineBounds = new Microsoft.Maui.Graphics.Rect(baselineRect.X1, baselineRect.Y1, baselineRect.Width, baselineRect.Height),
                                                    });

                                                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                                            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                    } while (iter.Next(PageIteratorLevel.Block));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                });
            }
            catch (System.Exception ex)
            {
                App.Current?.MainPage?.DisplayAlert("Error", ex.Message, "Cancel");
            }
            finally
            {
                activityIndicator.IsRunning = false;
            }
            return lines;
        }

        /// <summary>
        /// ChinesePunctuation2EnglishPunctuation
        /// </summary>
        static readonly Dictionary<char, char> c2ePuns = new Dictionary<char, char>()
        {
            {'。' ,'.'},
            {'，',',' },
            {'？','?' },
            {'“','"' },
            {'”','"' },
            {'！','!' },
            {'（','(' },
            {'）',')' },
            {'‘','\'' },
            {'’','\'' },
        };

        static readonly Dictionary<char, char> e2cPuns = new Dictionary<char, char>()
        {
            {'.','。' },
            {',' ,'，'},
            {'?','？'},
            {'"','"' },
            {'!','！' },
            {'(','（'},
            {')','）'},
            {'\'','\'' },
        };

        string ChinesePunctuation2EnglishPunctuation(string str)
        {
            var tmp = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                if (c2ePuns.ContainsKey(str[i]))
                {
                    tmp.Append(c2ePuns[str[i]]);
                }
                else
                {
                    tmp.Append(str[i]);
                }
            }
            return tmp.ToString();
        }
        string EnglishPunctuation2Chineseunctuation(string str)
        {
            var tmp = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                if (e2cPuns.ContainsKey(str[i]))
                {
                    tmp.Append(e2cPuns[str[i]]);
                }
                else
                {
                    tmp.Append(str[i]);
                }
            }
            return tmp.ToString();
        }

        /// <summary>
        /// 202209221200针对Tess对行文本会识别到错误的字符想到的切割单独行识别的方法
        /// 1.先识别一遍确定行宽高，切割图像再单独每行识别
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<List<OcrData>> RecognizeLines(MemoryStream stream, string lang = "chi_sim")
        {
            List<OcrData> GetLineOcrData(MemoryStream image, TesseractEngine tesseractEngine)
            {
                List<OcrData> lines = new List<OcrData>();
                try
                {
                    using (var img = Pix.LoadFromMemory(image.ToArray())) //LoadFromFile(testImagePath))
                    {
                        using (var page = tesseractEngine.Process(img))
                        {
                            //Debug.WriteLine(page.GetText());
                            //Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();
                                var language = iter.GetWordRecognitionLanguage();
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            //行
                                            var lineLevel = PageIteratorLevel.TextLine;
                                            iter.TryGetBoundingBox(lineLevel, out var lineRect);
                                            iter.TryGetBaseline(lineLevel, out var lineBaselineRect);
                                            var lineText = iter.GetText(lineLevel).Replace('\n', ' ');
                                            var line = new OcrData()
                                            {
                                                Text = lineText,
                                                Childs = new List<OcrData>(),
                                                Bounds = new Microsoft.Maui.Graphics.Rect(lineRect.X1, lineRect.Y1, lineRect.Width, lineRect.Height),
                                                BaselineBounds = new Microsoft.Maui.Graphics.Rect(lineBaselineRect.X1, lineBaselineRect.Y1, lineBaselineRect.Width, lineBaselineRect.Height),
                                            };
                                            lines.Add(line);
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
                return lines;
            }

            (List<OcrData>,string) GetLineWordsOcrData(MemoryStream image, TesseractEngine tesseractEngine)
            {
                List<OcrData> words = new List<OcrData>();
                string lineText = "";
                try
                {
                    using (var img = Pix.LoadFromMemory(image.ToArray())) //LoadFromFile(testImagePath))
                    {
                        using (var page = tesseractEngine.Process(img))
                        {
                            lineText = page.GetText();
                            //Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();
                                var language = iter.GetWordRecognitionLanguage();
                                //do
                                //{
                                //    do
                                //    {
                                //        do
                                //        {
                                do
                                {

                                    //行
                                    var wordLevel = PageIteratorLevel.Word;
                                    iter.TryGetBoundingBox(wordLevel, out var wordRect);
                                    iter.TryGetBaseline(PageIteratorLevel.TextLine, out var lineBaselineRect);
                                    var wordText = iter.GetText(wordLevel);
                                    if (wordText != null)
                                        wordText = wordText.Replace('\n', ' ');
                                    var word = new OcrData()
                                    {
                                        Text = wordText,
                                        Bounds = new Microsoft.Maui.Graphics.Rect(wordRect.X1, wordRect.Y1, wordRect.Width, wordRect.Height),
                                        BaselineBounds = new Microsoft.Maui.Graphics.Rect(lineBaselineRect.X1, lineBaselineRect.Y1, lineBaselineRect.Width, lineBaselineRect.Height),
                                    };
                                    words.Add(word);
                                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                                //        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                //    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                //} while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
                return (words,lineText);
            }

            List<OcrData> lines = null;
            try
            {
                activityIndicator.Dispatcher.Dispatch(() => activityIndicator.IsRunning = true);
                await Task.Run(() =>
                {
                    if (Directory.Exists(modelFolderPath))
                    {
                        if (!(new DirectoryInfo(modelFolderPath).GetFileSystemInfos().Length > 0))
                        {
                            InitTesseractDataAsync();
                        }
                    }
                    else
                        InitTesseractDataAsync();
                    if (tesseractEngine == null)
                        tesseractEngine = new TesseractEngine(modelFolderPath, lang, EngineMode.Default);
                    //第一次获取行区域
                    lines = GetLineOcrData(stream, tesseractEngine);
                    //切割
                    var sourceBigBitmap = SKBitmap.Decode(stream);

                    for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                    {
                        var line = lines[lineIndex];
                        //分析相邻的行,把行间距加入裁剪的图片
                        int topAdjacentLineIndex = lineIndex;
                        double topAdjacentSpaceHeight = 1000;
                        int bottomAdjacentLineIndex = lineIndex;
                        double bottomAdjacentSpaceHeight = 1000;
                        for (var index = 1; index < 5 && !(lineIndex - index <= 0 && lineIndex + index >= lines.Count); index++)
                        {
                            if (lineIndex - index > 0)
                            {
                                var newTopAdjacentHeight = line.Bounds.Top - lines[lineIndex - index].Bounds.Bottom;
                                if (newTopAdjacentHeight >= 0 && newTopAdjacentHeight < topAdjacentSpaceHeight)
                                {
                                    topAdjacentLineIndex = lineIndex - index;
                                    topAdjacentSpaceHeight = newTopAdjacentHeight;
                                }
                            }
                            if (lineIndex == 0) topAdjacentSpaceHeight = 15;
                            if (lineIndex + index < lines.Count)
                            {
                                var newBottomAdjacentHeight = lines[lineIndex + index].Bounds.Top - line.Bounds.Bottom;
                                if (newBottomAdjacentHeight >= 0 && newBottomAdjacentHeight < bottomAdjacentSpaceHeight)
                                {
                                    bottomAdjacentSpaceHeight = newBottomAdjacentHeight;
                                }
                            }
                            if (lineIndex == lines.Count - 1) bottomAdjacentSpaceHeight = 15;
                        }
                        if (topAdjacentSpaceHeight == 1000) topAdjacentSpaceHeight = 0;
                        if (bottomAdjacentSpaceHeight == 1000) bottomAdjacentSpaceHeight = 0;
                        //宽度两边也增加
                        int horizentalSpace = 12;
                        using (var cropBitmap = new SKBitmap((int)line.Bounds.Width + 2 * horizentalSpace, (int)(line.Bounds.Height + topAdjacentSpaceHeight + bottomAdjacentSpaceHeight)))
                        {
                            using (var canvas = new SKCanvas(cropBitmap))
                            {
                                canvas.Translate(-(float)(line.Bounds.X - horizentalSpace), -(float)(line.Bounds.Y - topAdjacentSpaceHeight));
                                canvas.DrawBitmap(sourceBigBitmap, 0, 0);
                            }
                            var newResult = GetLineWordsOcrData(cropBitmap.SKBitmapToStream(), tesseractEngine);
                            var words = newResult.Item1;
                            line.Text = newResult.Item2;//替换原来识别的
                            if (words != null)
                                line.Childs = words;//words数据用来绘制验证
                            using (var canvas = new SKCanvas(cropBitmap))
                            {
                                using (var paint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Red })
                                    foreach (var word in words)
                                    {
                                        canvas.DrawRect((float)word.Bounds.X, (float)word.Bounds.Y, (float)word.Bounds.Width, (float)word.Bounds.Height, paint);
                                    }
                            }
                            SaveService.Save(cropBitmap, $"{lineIndex}.png");
                        }
                    }
                });
            }
            catch (System.Exception ex)
            {
                App.Current?.MainPage?.DisplayAlert("Error", ex.Message, "Cancel");
            }
            finally
            {
                activityIndicator.IsRunning = false;
            }

            return lines;
        }
    }
}
#endif