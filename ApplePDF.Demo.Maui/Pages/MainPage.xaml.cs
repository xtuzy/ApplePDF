using ApplePDF.Demo.Maui.Extension;
using ApplePDF.PdfKit;
using PDFiumCore;
using SharpConstraintLayout.Maui.Widget;
using SkiaSharp;
using System.Text;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace ApplePDF.Demo.Maui
{
    public partial class MainPage : ContentPage
    {
        private PdfDocument doc;
        private ConstraintLayout WindowPage;
        private HorizontalStackLayout buttonContainer;
        private Button SelectFileButton;
        private Button ShowPdfButton;
        private Button GetTextButton;
        private ActivityIndicator GetTextActivityIndicator;
        private Label PageScaleLable;
        private Entry PageScaleTextBox;
        private Entry PageCurrentIndexEntry;
        private Label PageCurrentToLastIndexLabel;
        private Label PageLastIndexLable;
        private ListView DocTreeView;
        private ScrollView PageScrollViewer;
        private Image PageImage;
        string defaultDoc = "pdfBible.pdf";
        public MainPage()
        {
            InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
            ConstraintLayout.DEBUG = true;
            WindowPage = new ConstraintLayout() { BackgroundColor = Colors.DarkGray };
            Content = WindowPage;

            buttonContainer = new HorizontalStackLayout();
            var catalogManagerButton = new Button() { Text = "目录", WidthRequest = 35, HeightRequest = 20, Padding = new Thickness(0, 0, 0, 0), CornerRadius = 0 };
            SelectFileButton = new Button() { Text = "选择", WidthRequest = 35, HeightRequest = 20, Padding = new Thickness(0, 0, 0, 0), CornerRadius = 0 };
            ShowPdfButton = new Button() { Text = "打开", WidthRequest = 35, HeightRequest = 20, Padding = new Thickness(0, 0, 0, 0), CornerRadius = 0 };
            GetTextButton = new Button() { Text = "文本", WidthRequest = 35, HeightRequest = 20, Padding = new Thickness(0, 0, 0, 0), CornerRadius = 0 };
            GetTextActivityIndicator = new ActivityIndicator() { IsRunning = false };
            buttonContainer.AddViews(catalogManagerButton, SelectFileButton, ShowPdfButton, GetTextButton, GetTextActivityIndicator);
            PageScaleLable = new Label() { Text = "缩放图片精度" };
            PageScaleTextBox = new Entry() { Text = "1", VerticalTextAlignment = TextAlignment.Center };
            PageCurrentIndexEntry = new Entry() { Text = "1" };
            PageCurrentToLastIndexLabel = new Label() { Text = "/" };
            PageLastIndexLable = new Label() { Text = "1" };

            DocTreeView = new ListView();
            PageScrollViewer = new ScrollView() { };
            PageImage = new Image() { };

            WindowPage.AddElement(buttonContainer);
            WindowPage.AddElement(PageScaleLable);
            WindowPage.AddElement(PageScaleTextBox);
            WindowPage.AddElement(PageCurrentIndexEntry);
            WindowPage.AddElement(PageCurrentToLastIndexLabel);
            WindowPage.AddElement(PageLastIndexLable);
            WindowPage.AddElement(DocTreeView);
            WindowPage.AddElement(PageScrollViewer);

            PageScrollViewer.Content = PageImage;
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
            ShowPdfButton.Clicked += ShowPdfButton_Clicked;
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
                    ShowPdfButton_Clicked(null, null);
                }
            };
            InitPdfLibrary();
            ReadPDFAsyncFormResourcesAsync();
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
                //if (System.OperatingSystem.IsWindows())
                if (this.Width > 500)
                {
                    set.Select(buttonContainer).LeftToLeft(null, 20).TopToTop(null, 10)
                        .Select(PageScaleLable).Clear().LeftToRight(buttonContainer, 5).CenterYTo(buttonContainer)
                        .Select(PageScaleTextBox).Clear().MinWidth(30).LeftToRight(PageScaleLable, 5).CenterYTo(buttonContainer)
                        .Select(PageCurrentIndexEntry).Clear().MinWidth(30).RightToLeft(PageCurrentToLastIndexLabel, 5).CenterYTo(buttonContainer)
                        .Select(PageCurrentToLastIndexLabel).Clear().LeftToRight(PageScaleTextBox).RightToRight().CenterYTo(buttonContainer)
                        .Select(PageLastIndexLable).Clear().LeftToRight(PageCurrentToLastIndexLabel).CenterYTo(buttonContainer)
                        .Select(DocTreeView).ClearEdges().LeftToLeft(buttonContainer).Margin(Edge.Right, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5).Width(200).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageScrollViewer).LeftToRight(DocTreeView).RightToRight(null, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        ;
                }
                else
                {
                    set.Select(buttonContainer).L2L(null, 20).T2T(null, 10)
                        .Select(PageScaleLable).Clear().L2L(buttonContainer).CenterYTo(PageScaleTextBox)
                        .Select(PageScaleTextBox).Clear().MinWidth(30).L2R(PageScaleLable, 5).T2B(buttonContainer, 5)
                        .Select(PageCurrentIndexEntry).Clear().MinWidth(30).R2L(PageCurrentToLastIndexLabel, 5).CenterYTo(PageScaleLable)
                        .Select(PageCurrentToLastIndexLabel).Clear().L2R(PageScaleTextBox).R2R().CenterYTo(PageCurrentIndexEntry)
                        .Select(PageLastIndexLable).Clear().L2R(PageCurrentToLastIndexLabel).CenterYTo(PageCurrentIndexEntry)
                        .Select(DocTreeView).ClearEdges().L2L(buttonContainer).T2B(PageCurrentIndexEntry, 5)//.BottomToBottom()
                        .Width(200).Height(SizeBehavier.WrapContent)
                        .Select(PageScrollViewer).Clear().L2R(DocTreeView).R2R(null, 20).T2B(PageCurrentIndexEntry, 5).B2B(null, 5)
                        .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint);

                }
                set.ApplyTo(WindowPage);
            }
        }

        private void ShowPdfButton_Clicked(object sender, EventArgs e)
        {
            var index = int.Parse(PageCurrentIndexEntry.Text) - 1;
            if (index < 0) index = 0;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);

            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            using var page = doc.GetPage(index);
            MemoryStream stream = null;
            using (var bitmap = PdfPageExtension.RenderPageToSKBitmapFormSKBitmap(page, scale, flags))
                stream = bitmap.SKBitmapToStream();

            PageImage.Source = ImageSource.FromStream(() => stream);

            WindowPage.RequestReLayout();
        }

        private async void SelectFileButton_ClickedAsync(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(PickOptions.Default);
            if (result == null)
                return;
            ReadPDFAsync(result.FullPath);
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

        MemoryStream memoryStream = new MemoryStream();

        async Task ReadPDFAsyncFormResourcesAsync(string filePath = null)
        {

            if (filePath == null)
            {
                await FileSystem.OpenAppPackageFileAsync(defaultDoc).ContinueWith(t =>
                {
                    t.Result.CopyTo(memoryStream);
                    if (doc != null)
                        doc.Dispose();
                    doc = Pdfium.Instance.LoadPdfDocument(memoryStream, null);

                    //PageIndexSlider.InvalidateVisual();
                    var rootBookmark = doc.OutlineRoot;
                    List<string> bookmarks = new List<string>();
                    if (rootBookmark != null)
                    {
                        // Debug.WriteLine(rootBookmark.Label);
                        bookmarks.Add(rootBookmark.Label);
                        foreach (var child in rootBookmark.Children)
                        {
                            // Debug.WriteLine(child.Label);
                            bookmarks.Add(child.Label);
                            foreach (var child2 in child.Children)
                            {
                                //  Debug.WriteLine(child2.Label);
                                bookmarks.Add(child2.Label);
                            }
                        }
                    }
                    WindowPage.Dispatcher.Dispatch(() =>
                    {
                        PageLastIndexLable.Text = $"{doc.PageCount}";
                        DocTreeView.ItemsSource = bookmarks;
                        WindowPage.RequestReLayout();
                    });
                });
            }

        }

        void ReadPDFAsync(string filePath = null)
        {
            Stream stream = null;
            if (filePath == null)
            {
                return;
                //stream = await FileSystem.OpenAppPackageFileAsync("AboutAssets.txt");
            }
            stream = File.OpenRead(filePath);
            if (doc != null)
                doc.Dispose();
            doc = Pdfium.Instance.LoadPdfDocument(stream, null);
            PageLastIndexLable.Text = $"{doc.PageCount}";
            //PageIndexSlider.InvalidateVisual();
            var rootBookmark = doc.OutlineRoot;
            List<string> bookmarks = new List<string>();
            if (rootBookmark != null)
            {
                // Debug.WriteLine(rootBookmark.Label);
                bookmarks.Add(rootBookmark.Label);
                foreach (var child in rootBookmark.Children)
                {
                    // Debug.WriteLine(child.Label);
                    bookmarks.Add(child.Label);
                    foreach (var child2 in child.Children)
                    {
                        //  Debug.WriteLine(child2.Label);
                        bookmarks.Add(child2.Label);
                    }
                }
            }
            DocTreeView.ItemsSource = bookmarks;
        }
    }
}