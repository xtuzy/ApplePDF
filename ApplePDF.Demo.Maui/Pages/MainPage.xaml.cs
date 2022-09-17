using ApplePDF.Demo.Maui.Extension;
using ApplePDF.PdfKit;
using PDFiumCore;
using SharpConstraintLayout.Maui.Widget;
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
        private Label PageFirstIndexLable;
        private Slider PageIndexSlider;
        private Label PageLastIndexLable;
        private Label PageScaleLable;
        private Entry PageScaleTextBox;
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
            buttonContainer.AddViews(catalogManagerButton, SelectFileButton, ShowPdfButton, GetTextButton);
            PageScaleLable = new Label() { Text = "放大倍数" };
            PageScaleTextBox = new Entry() { Text = "1" };
            PageFirstIndexLable = new Label() { Text = "1" };
            PageIndexSlider = new Slider() { Value = 1 };
            PageLastIndexLable = new Label() { Text = "1" };

            DocTreeView = new ListView();
            PageScrollViewer = new ScrollView() { };
            PageImage = new Image() { };

            WindowPage.AddElement(buttonContainer);
            WindowPage.AddElement(PageScaleLable);
            WindowPage.AddElement(PageScaleTextBox);
            WindowPage.AddElement(PageFirstIndexLable);
            WindowPage.AddElement(PageIndexSlider);
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
            PageIndexSlider.ValueChanged += (sender, e) =>
            {
                ShowPdfButton_Clicked(null, null);
            };
            InitPdfLibrary();
            ReadPDFAsyncFormResourcesAsync();
        }

        private void GetTextButton_Clicked(object sender, EventArgs e)
        {
            var index = (int)PageIndexSlider.Value - 1;
            if (index < 0) index = 0;
            var page = doc.GetPage(index);
            page.GetCharacterBounds
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
                        .Select(PageScaleLable).LeftToRight(buttonContainer, 20).CenterYTo(buttonContainer)
                        .Select(PageScaleTextBox).LeftToRight(PageScaleLable, 20).CenterYTo(buttonContainer)
                        .Select(PageFirstIndexLable).LeftToRight(PageScaleTextBox, 20).CenterYTo(buttonContainer)
                        .Select(PageIndexSlider).LeftToRight(PageFirstIndexLable, 5).CenterYTo(buttonContainer).RightToLeft(PageLastIndexLable, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageLastIndexLable).RightToRight(null, 20).CenterYTo(buttonContainer)
                        .Select(DocTreeView).ClearEdges().LeftToLeft(buttonContainer).Margin(Edge.Right, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5).Width(200).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageScrollViewer).LeftToRight(DocTreeView).RightToRight(null, 20).TopToBottom(buttonContainer, 5).BottomToBottom(null, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        ;
                }
                else
                {
                    set.Select(buttonContainer).L2L(null, 20).T2T(null, 10)
                        .Select(PageScaleLable).L2R(buttonContainer, 5).CenterYTo(buttonContainer)
                        .Select(PageScaleTextBox).L2R(PageScaleLable).CenterYTo(buttonContainer)
                        .Select(PageFirstIndexLable).Clear().L2L(null, 20).T2B(buttonContainer, 5)
                        .Select(PageIndexSlider).Clear().L2R(PageFirstIndexLable, 5).CenterYTo(PageFirstIndexLable).R2L(PageLastIndexLable, 5)
                        .Width(SizeBehavier.MatchConstraint)
                        .Select(PageLastIndexLable).Clear().L2R(PageIndexSlider, 5).R2R(null, 20).CenterYTo(PageFirstIndexLable)
                        .Select(DocTreeView).ClearEdges().L2L(buttonContainer).T2B(PageFirstIndexLable, 5)//.BottomToBottom()
                        .Width(200).Height(SizeBehavier.WrapContent)
                        .Select(PageScrollViewer).Clear().L2R(DocTreeView).R2R(null, 20).T2B(PageFirstIndexLable, 5).B2B(null, 5)
                        .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint);

                }
                set.ApplyTo(WindowPage);
            }
        }

        private void ShowPdfButton_Clicked(object sender, EventArgs e)
        {
            var index = (int)PageIndexSlider.Value - 1;
            if (index < 0) index = 0;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);

            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            var stream = PdfPageExtension.RenderPageToSKBitmapFormSKBitmap(doc.GetPage(index), scale, flags).SKBitmapToStream();

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
                        PageIndexSlider.Maximum = doc.PageCount;
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
            PageIndexSlider.Maximum = doc.PageCount;
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