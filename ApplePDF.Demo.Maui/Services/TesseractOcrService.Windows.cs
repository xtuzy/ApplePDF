#if WINDOWS
using ApplePDF.Demo.Maui.Extension;
using MathNet.Numerics.Statistics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
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
            //Ocr行和Words
            async Task<List<OcrData>> StandardOcrAsync(MemoryStream stream, string lang)
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
            List<OcrData> lines = await StandardOcrAsync(stream, lang);
            /* 
             * Tess初步识别的坐标需要修正
             */
            //1.行矩形是一般比较标准的，但有极端情况，如黑色背景时可能会识别整个黑色区域为行，我们需要修正和重新识别
            //2.标准的行矩形可以作为文字大小参考，需要考虑宽高。
            //3.单个文字区域重复需要考虑区域偏移，左右结构汉字区域错误。一般可以从后往前推。
            //先实现3
            void FixWordBoundsHorizontalOffset(List<OcrData> lines)
            {
                float spaceWithWidthFixCoefficient = 1f;//用于修正字的宽和间距
                
                foreach (var line in lines)
                {
                    double lineHeght = line.Bounds.Height;
                    for (var index = 0; index < line.Childs.Count; index++)
                    {
                        var currentWord = line.Childs[index];
                        if ('a' < currentWord.Text[0] &&
                            currentWord.Text[0] < 'z'&&
                            'A' < currentWord.Text[0] &&
                            currentWord.Text[0] < 'Z')//英文不适用
                            continue;
                        //修正全部过宽的Word
                        if (currentWord.Bounds.Width > lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient)
                        {
                            currentWord.Bounds.Width = lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;
                        }

                        var currentWordWidthIsTooBig = currentWord.Bounds.Width > lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;//估计是否包含多个中文单词的宽度
                        
                        if (index + 1 < line.Childs.Count)
                        {
                            var nextWord = line.Childs[index + 1];
                            var nextWordWidthIsTooBig = nextWord.Bounds.Width > lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;//估计是否包含多个中文单词的宽度
                            //当前word与后一个word对比，分析宽度关系

                            if (currentWord.Bounds.Left < nextWord.Bounds.Left
                                && currentWord.Bounds.Right > nextWord.Bounds.Left)//若当前word右侧包含next的一部分
                            {
                                /*
                                 * |  curr       |
                                 *   |   next  |
                                 */
                                if (!nextWordWidthIsTooBig)//next宽度是标准的，那么一般是在对的位置
                                {
                                    /*
                                     * |  curr      |
                                     *       |next|
                                     */
                                    currentWord.Bounds.Right = nextWord.Bounds.Left - 1;//缩小current范围到next左边
                                    //如果右侧缩小后还是太大
                                    if (currentWord.Bounds.Width > lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient)
                                    {
                                        currentWord.Bounds.Width = lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;
                                    }
                                }
                                else
                                {
                                    if (nextWord.Bounds.Left - currentWord.Bounds.Left > lineHeght * currentWord.Text.Length * 0.8) //如果留出足够的空间放current
                                    {
                                        /*
                                         * |  curr         |
                                         *  ****|   next  |
                                         */
                                        currentWord.Bounds.Right = nextWord.Bounds.Left - 1;//缩小当前word范围
                                    }
                                    else//没有留够足够空间时，我们尝试为当前字符腾一个字符宽度出来
                                    {
                                        /*
                                         * |  curr        |
                                         *  **|   next  |
                                         */
                                        currentWord.Bounds.Width = lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;//先把宽度正常化
                                        nextWord.Bounds.Left = currentWord.Bounds.Right + 1;//再移动下一个word
                                    }
                                }
                            }
                            else if (currentWord.Bounds.Left > nextWord.Bounds.Left) //如果当前左侧超过了下一个单词左侧
                            {
                                /*
                                 *    |  curr        |
                                 * |   next  |
                                 */
                                if (!currentWordWidthIsTooBig)//当前宽度正常
                                {
                                    /*
                                     *    |curr|
                                     * |   next  |
                                     */
                                    nextWord.Bounds.Left = currentWord.Bounds.Right + 1;
                                }
                                else
                                {
                                    /*
                                     *    |  curr  |
                                     * |   next  |
                                     */
                                    currentWord.Bounds.Width = lineHeght * currentWord.Text.Length * spaceWithWidthFixCoefficient;//先把宽度正常化
                                    nextWord.Bounds.Left = currentWord.Bounds.Right + 1;
                                }
                            }
                        }
                    }
                }
            }
#if DEBUG
            SKBitmap PrintImageDebug(MemoryStream stream, List<OcrData> lines)
            {
                var array = stream.ToArray();
                var sourceBitmap = SKBitmap.Decode(array);
                //var sourceBitmap = SKBitmap.Decode(stream);
                //绘制Word边框
                using (var canvas = new SKCanvas(sourceBitmap))
                {
                    using (var paint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Red })
                    {
                        foreach (var line in lines)
                        {
                            foreach (var word in line.Childs)
                            {
                                canvas.DrawRect((float)word.Bounds.X, (float)word.Bounds.Y, (float)word.Bounds.Width, (float)word.Bounds.Height, paint);
                            }
                        }
                    }
                }
                return sourceBitmap;
            }
            var source = PrintImageDebug(stream, new List<OcrData>());
            SaveService.Save(source, $"{nameof(RecognizeWords)}.Source.Debug.png");
            var sourceWithRectBitmap = PrintImageDebug(stream, lines);
            SaveService.Save(sourceWithRectBitmap, $"{nameof(RecognizeWords)}.SourceWithRect.Debug.png");
#endif
            FixWordBoundsHorizontalOffset(lines);
#if DEBUG
            var fixedBitmap = PrintImageDebug(stream, lines);
            SaveService.Save(fixedBitmap, $"{nameof(RecognizeWords)}.Fixed.Debug.png");
#endif
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
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<List<OcrData>> RecognizeLines(MemoryStream stream, string lang = "chi_sim")
        {
            //获取所有Line的数据
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

            //获取局部图片中的Words，可能存在多行
            (List<OcrData>, string) GetLineWordsOcrData(MemoryStream image, TesseractEngine tesseractEngine)
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
                                do
                                {
                                    do
                                    {
                                        do
                                        {
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
                return (words, lineText);
            }

            List<OcrData> lines = null;
            Dictionary<OcrData, List<OcrData>> needInsteadLines = new Dictionary<OcrData, List<OcrData>>();
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
                        //TODO:这里的区域改变了，如果遇到之前整体识别时有没有识别到的，那么这里就会覆盖，所以要考虑替换已存在在该区域内的行
                        using (var cropBitmap = new SKBitmap((int)line.Bounds.Width + 2 * horizentalSpace, (int)(line.Bounds.Height + topAdjacentSpaceHeight + bottomAdjacentSpaceHeight)))
                        {
                            using (var canvas = new SKCanvas(cropBitmap))
                            {
                                canvas.Translate(-(float)(line.Bounds.X - horizentalSpace), -(float)(line.Bounds.Y - topAdjacentSpaceHeight));
                                canvas.DrawBitmap(sourceBigBitmap, 0, 0);
                            }
                            var newResult = GetLineWordsOcrData(cropBitmap.SKBitmapToStream(), tesseractEngine);
                            var words = newResult.Item1;
#if DEBUG
                            //打印每行的图片
                            using (var canvas = new SKCanvas(cropBitmap))
                            {
                                using (var paint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Red })
                                    foreach (var word in words)
                                    {
                                        canvas.DrawRect((float)word.Bounds.X, (float)word.Bounds.Y, (float)word.Bounds.Width, (float)word.Bounds.Height, paint);
                                    }
                            }
                            SaveService.Save(cropBitmap, $"{lineIndex}.png");
#endif
                            if (line.Text != newResult.Item2)//替换原来识别的
                            {
                                //存储时图片区域纠正
                                foreach (var word in words)
                                {
                                    word.Bounds.X = word.Bounds.X + line.Bounds.X - horizentalSpace;
                                    word.Bounds.Y = word.Bounds.Y + line.Bounds.Y - topAdjacentSpaceHeight;
                                    word.BaselineBounds.X = word.BaselineBounds.X + line.Bounds.X - horizentalSpace;
                                    word.BaselineBounds.Y = word.BaselineBounds.Y + line.Bounds.Y - topAdjacentSpaceHeight;
                                }
                                line.Childs = words;//words数据用来绘制验证
                                needInsteadLines.Add(line, newResult.Item1);
                            }
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
            foreach (var needInstead in needInsteadLines)
            {
                lines.Remove(needInstead.Key);
                lines.AddRange(needInstead.Value);
            }
            return lines;
        }
    }
}
#endif