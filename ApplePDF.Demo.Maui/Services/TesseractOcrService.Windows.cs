#if WINDOWS
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

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
        /// Recognizes the text by Tess.比windows自带的慢
        /// </summary>
        /// <param name="stream">must The memerystream.</param>
        /// <param name="modelName">chi_sim,chi_sim_vert,chi_tra</param>
        /// <returns></returns>
        public async Task<List<OcrData>> RecognizeWords(MemoryStream stream, string lang = "chi_sim")
        {
            List<OcrData> lines = new List<OcrData>();
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
                                Debug.WriteLine(page.GetText());
                                //Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                                using (var iter = page.GetIterator())
                                {
                                    iter.Begin();

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
                                                var line = new OcrData()
                                                {
                                                    Childs = new List<OcrData>(),
                                                    Text = iter.GetText(lineLevel).Replace('\n', ' '),
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
    }
}
#endif