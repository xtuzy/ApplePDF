using ApplePDF.Demo.Maui.Extension;
using ApplePDF.Demo.Maui.Services;
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
            GetWordsButton.Clicked += GetWordsButton_Clicked;
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
            var ocrLines = await new ApplePDF.Demo.Maui.Services.TesseractOcrService(GetTextActivityIndicator).RecognizeWords(stream);

            OcrResultProcess.DrawLineRect(pdfImage, ocrLines);
            var size = page.GetSize();
            using var bitmap = new SKBitmap((int)(size.Width * scale), (int)(size.Height * scale));
            OcrResultProcess.DrawLineRect(bitmap, ocrLines);
            OcrResultProcess.DrawLineText(bitmap, ocrLines);
            SaveService.Save(pdfImage, "Pdf.png");
            SaveService.Save(bitmap, "Result.png");
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

            OcrResultProcess.DrawLineRect(pdfImage, ocrLines);
            
            var size = page.GetSize();
            using var bitmap = new SKBitmap((int)(size.Width * scale), (int)(size.Height * scale));
            OcrResultProcess.DrawLineRect(bitmap, ocrLines);
            
            OcrResultProcess.DrawLineTextWithFixPosition(bitmap, ocrLines);

            SaveService.Save(pdfImage, "Pdf.png");
            SaveService.Save(bitmap, "Result.png");
#endif
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