using ApplePDF.Demo.Maui.Extension;
using ApplePDF.PdfKit;
using CommunityToolkit.Mvvm.ComponentModel;
using PDFiumCore;
using SharpConstraintLayout.Maui.Widget;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Text;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace ApplePDF.Demo.Maui
{
    public partial class MainPage : ContentPage
    {
        private PdfDocument doc;
        string defaultDoc = "pdfBible.pdf";
        public MainPage()
        {
            InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;

            catalogManagerButton.Clicked += (sender, e) =>
            {
                if (DocTreeView.IsVisible)
                    using (var set = new FluentConstraintSet())
                    {
                        set.Clone(WindowPage);
                        set.Select(DocTreeView).Visibility(FluentConstraintSet.Visibility.Gone);
                        set.ApplyTo(WindowPage);
                    }
                else
                {
                    using (var set = new FluentConstraintSet())
                    {
                        set.Clone(WindowPage);
                        set.Select(DocTreeView).Visibility(FluentConstraintSet.Visibility.Visible);
                        set.ApplyTo(WindowPage);
                    }
                }
            };

            SelectFileButton.Clicked += SelectFileButton_ClickedAsync;
            GetTextButton.Clicked += GetTextButton_Clicked;
            int lastPageIndex = 1;
            PageCurrentIndexEntry.Completed += (sender, e) =>
            {
                var newPageIndex = int.Parse(PageCurrentIndexEntry.Text);
                if (newPageIndex > doc.PageCount || newPageIndex < 1)
                {
                    PageCurrentIndexEntry.Text = lastPageIndex.ToString();
                }
                else
                {
                    lastPageIndex = newPageIndex;
                    ShowPdf();
                }
            };
            InitPdfLibrary();
            SelectPdfFormResourcesAsync();
        }

        private async void GetTextButton_Clicked(object sender, EventArgs e)
        {
            GetWordsButton_Clicked(sender, e);
            return;

            var index = int.Parse(PageCurrentIndexEntry.Text) - 1;
            if (index < 0) index = 0;
            using var page = doc.GetPage(index);
            //var text = page.Text;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);

            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            MemoryStream stream = null;
            using var pdfImage = PdfPageExtension.RenderPageToSKBitmapFormSKBitmap(doc.GetPage(index), scale, flags);
            stream = pdfImage.SKBitmapToStream();
#if WINDOWS
            //Windows自带OCR
            //var ocrText = await ApplePDF.Demo.Maui.Services.OcrService.RecognizeText(stream as Stream);
            //var ocrWords = await ApplePDF.Demo.Maui.Services.OcrService.RecognizeWords(stream as Stream);
            //用Tess来OCR
            //var ocrText = await ApplePDF.Demo.Maui.Services.OcrService.RecognizeText(stream);
            ///var ocrLines = await new ApplePDF.Demo.Maui.Services.TesseractOcrService(GetTextActivityIndicator).RecognizeWords(stream);
            var ocrLines = await new ApplePDF.Demo.Maui.Services.TesseractOcrService(GetTextActivityIndicator).RecognizeLines(stream);

            using (var canvas = new SKCanvas(pdfImage))
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
                        canvas.DrawRect((float)line.BaselineBounds.X, (float)line.BaselineBounds.Y, (float)line.BaselineBounds.Width, (float)line.BaselineBounds.Height, paint);
                    }
                }
            }
            var size = page.GetSize();
            using var bitmap = new SKBitmap((int)(size.Width * scale), (int)(size.Height * scale));
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);
                using (var paint = new SKPaint() { Color = SKColors.Black, Typeface = SKFontManager.Default.MatchCharacter('中') })
                {
                    //绘制文字轮廓背景,用来看文字区域是否识别正确
                    paint.Color = SKColors.Black.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawRect((float)line.Bounds.X, (float)line.Bounds.Y, (float)line.Bounds.Width, (float)line.Bounds.Height, paint);
                    }
                    //绘制基线局域,用来看文字区域是否识别正确
                    paint.Color = SKColors.Red.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawRect((float)line.BaselineBounds.X, (float)line.BaselineBounds.Y, (float)line.BaselineBounds.Width, (float)line.BaselineBounds.Height, paint);
                    }

                    //绘制文字
                    paint.Color = SKColors.AliceBlue;
                    var linesTextSize = Services.TesseractOcrService.AnalysisTextSize(ocrLines);
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

            save(pdfImage, "Pdf.png");
            save(bitmap, "Result.png");
#endif
        }

        private async void GetWordsButton_Clicked(object sender, EventArgs e)
        {
            var index = int.Parse(PageCurrentIndexEntry.Text) - 1;
            if (index < 0) index = 0;
            using var page = doc.GetPage(index);
            //var text = page.Text;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);

            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            MemoryStream stream = null;
            using var pdfImage = PdfPageExtension.RenderPageToSKBitmapFormSKBitmap(doc.GetPage(index), scale, flags);
            stream = pdfImage.SKBitmapToStream();
#if WINDOWS
            var ocrLines = await new ApplePDF.Demo.Maui.Services.TesseractOcrService(GetTextActivityIndicator).RecognizeLines(stream);

            using (var canvas = new SKCanvas(pdfImage))
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
                        canvas.DrawRect((float)line.BaselineBounds.X, (float)line.BaselineBounds.Y, (float)line.BaselineBounds.Width, (float)line.BaselineBounds.Height, paint);
                    }
                }
            }
            var size = page.GetSize();
            using var bitmap = new SKBitmap((int)(size.Width * scale), (int)(size.Height * scale));
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);
                using (var paint = new SKPaint() { Color = SKColors.Black, Typeface = SKFontManager.Default.MatchCharacter('中') })
                {
                    //绘制文字轮廓背景,用来看文字区域是否识别正确
                    paint.Color = SKColors.Black.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawRect((float)line.Bounds.X, (float)line.Bounds.Y, (float)line.Bounds.Width, (float)line.Bounds.Height, paint);
                    }
                    //绘制基线局域,用来看文字区域是否识别正确
                    paint.Color = SKColors.Red.WithAlpha(150);
                    foreach (var line in ocrLines)
                    {
                        canvas.DrawRect((float)line.BaselineBounds.X, (float)line.BaselineBounds.Y, (float)line.BaselineBounds.Width, (float)line.BaselineBounds.Height, paint);
                    }

                    //绘制文字
                    paint.Color = SKColors.AliceBlue;
                    var linesTextSize = Services.TesseractOcrService.AnalysisTextSize(ocrLines);
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

            save(pdfImage, "Pdf.png");
            save(bitmap, "Result.png");
#endif
        }

        public static void save(SKBitmap bitmap, string fileName)
        {
            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream))
            {
                bitmap.Encode(wstream, SKEncodedImageFormat.Png, 300);
                byte[] data = memStream.ToArray();

                if (data == null)
                {
                    throw new Exception("Encode returned null");
                }
                else if (data.Length == 0)
                {
                    throw new Exception("Encode returned empty array");
                }
                else
                {
                    try
                    {
                        var path = FileSystem.Current.AppDataDirectory;
                        using (var fs = new FileStream(Path.Combine(path, fileName), FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex) { throw new Exception("Save fail"); }
                }
            }
        }

        private void MainPage_SizeChanged(object sender, EventArgs e)
        {
            using (var set = new FluentConstraintSet())
            {
                set.Clone(WindowPage);
                set.Select(GetTextActivityIndicator).RightToRight(null, 20).CenterYTo(buttonContainer).Width(20).Height(20).MinWidth(20).MinHeight(20);
                //if (System.OperatingSystem.IsWindows())
                if (this.Width > 500)
                {
                    set.Select(buttonContainer).LeftToLeft(null, 20).TopToTop(null, 10)
                        .Select(PageIndexComponent).Clear().CenterXTo().CenterYTo(buttonContainer)
                        .Select(DocTreeView).ClearEdges().LeftToLeft(buttonContainer).Margin(Edge.Right, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5)
                        .Width(220).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageScrollViewer).LeftToLeft(DocTreeView).RightToRight(null, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        ;
                }
                else
                {
                    set.Select(buttonContainer).L2L(null, 20).T2T(null, 10)
                        .Select(PageIndexComponent).Clear().CenterXTo().TopToBottom(buttonContainer, 5)
                        .Select(DocTreeView).ClearEdges().L2L(buttonContainer).T2B(PageIndexComponent, 5)//.BottomToBottom()
                        .Width(220).Height(SizeBehavier.WrapContent)
                        .Select(PageScrollViewer).Clear().L2L(DocTreeView).R2R(null, 20).T2B(PageIndexComponent, 5).B2B(null, 5)
                        .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint);
                }
                set.ApplyTo(WindowPage);
            }
        }

        private void ShowPdf()
        {
            var index = int.Parse(PageCurrentIndexEntry.Text) - 1;
            if (index < 0) index = 0;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);

            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            using var page = doc.GetPage(index);
            MemoryStream stream = null;
            var bitmap = PdfPageExtension.RenderPageToSKBitmapFormSKBitmap(page, scale, flags);
            stream = bitmap.SKBitmapToStream();
            if (this.Dispatcher.IsDispatchRequired)
                this.Dispatcher.Dispatch(() =>
                {
                    PageImage.Source = null;
                    PageImage.Source = ImageSource.FromStream(() => stream);
                    WindowPage.RequestReLayout();
                });
            else
            {
                PageImage.Source = null;
                PageImage.Source = ImageSource.FromStream(() => stream);
                WindowPage.RequestReLayout();
            }
        }

#if ANDROID
        void save(PdfPage page)
        {
            var pageSize = page.GetSize();
            var androidBitmap = Android.Graphics.Bitmap.CreateBitmap((int)pageSize.Width, (int)pageSize.Height, Android.Graphics.Bitmap.Config.Argb8888);
            ApplePDF.Maui.Extensions.PdfPageExtension.RenderPage(page, androidBitmap, (float)DeviceDisplay.MainDisplayInfo.Density
                , 0, 0, androidBitmap.Width, androidBitmap.Height, false);
            var filePath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "result.jpeg");
            ApplePDF.Maui.Extensions.PdfPageExtension.WriteBitmapToFile(filePath, androidBitmap, 100);
        }
#endif

        private async void SelectFileButton_ClickedAsync(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(PickOptions.Default);
            if (result == null)
                return;
            if (result.FullPath == null)
            {
                return;
            }
            Stream stream = File.OpenRead(result.FullPath);
            ReadPdfAsync(stream);
            ShowPdf();
        }

        void InitPdfLibrary()
        {
            try
            {
                fpdfview.FPDF_InitLibrary();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task SelectPdfFormResourcesAsync(string filePath = null)
        {
            if (filePath == null)
            {
                await FileSystem.OpenAppPackageFileAsync(defaultDoc).ContinueWith(t =>
                {
                    MemoryStream memoryStream = new MemoryStream();
                    t.Result.CopyTo(memoryStream);
                    ReadPdfAsync(memoryStream);
                    ShowPdf();
                });
            }
        }

        void ReadPdfAsync(Stream stream)
        {
            if (doc != null)
                doc.Dispose();
            doc = Pdfium.Instance.LoadPdfDocument(stream, null);
            var rootBookmark = doc.OutlineRoot;
            var viewModel = new DocTreeViewModel();
            var bookmarks = viewModel.Bookmarks;
            if (rootBookmark != null)
            {
                // Debug.WriteLine(rootBookmark.Label);
                //bookmarks.Add(new DocTreeModel() { Title = rootBookmark.Label });
                //添加一级书签
                foreach (var child in rootBookmark.Children)
                {
                    // Debug.WriteLine(child.Label);
                    var firstLevelBookmark = new DocTreeModel() { Name = child.Label };
                    bookmarks.Add(firstLevelBookmark);
                    //添加二级书签
                    foreach (var child2 in child.Children)
                    {
                        //  Debug.WriteLine(child2.Label);
                        firstLevelBookmark.Children.Add(new DocTreeModel() { Name = child2.Label });
                    }
                }
            }
            this.Dispatcher.Dispatch(() =>
            {
                PageLastIndexLable.Text = $"{doc.PageCount}";
                this.BindingContext = viewModel;
                bookmarksView.ItemsSource = viewModel.Bookmarks;
                WindowPage.RequestReLayout();
            });
        }

        bool IsDark = true;
        private void SwitchDayAndLightButton_Clicked(object sender, EventArgs e)
        {
            if (IsDark)
            {
                PageScrollViewer.BackgroundColor = Colors.White;
                IsDark = false;
            }
            else
            {
                PageScrollViewer.BackgroundColor = Color.Parse("#33333");
                IsDark = true;
            }
        }
    }

    public class DocTreeModel
    {
        public virtual string Name { get; set; }
        public int PageIndex { get; set; }
        public virtual IList<DocTreeModel> Children { get; set; } = new ObservableCollection<DocTreeModel>();
    }

    public partial class DocTreeViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<DocTreeModel> bookmarks;

        public DocTreeViewModel()
        {
            bookmarks = new ObservableCollection<DocTreeModel>();
        }
    }
}