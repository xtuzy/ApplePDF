using ApplePDF.Demo.Maui.Extension;
using ApplePDF.PdfKit;
using PDFiumCore;
using SharpConstraintLayout.Maui.Widget;
using System.IO;

namespace ApplePDF.Demo.Maui
{
    public partial class MainPage : ContentPage
    {
        private PdfDocument doc;
        private ConstraintLayout WindowPage;
        private Button SelectFileButton;
        private Button ShowPdfButton;
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
            ConstraintLayout.DEBUG = true;
            WindowPage = new ConstraintLayout() { BackgroundColor = Colors.DarkGray };
            Content = WindowPage;

            SelectFileButton = new Button() { Text = "选择" };
            ShowPdfButton = new Button() { Text = "打开" };
            PageScaleLable = new Label() { Text = "放大倍数" };
            PageScaleTextBox = new Entry() { Text = "1" };
            PageFirstIndexLable = new Label() { Text = "1" };
            PageIndexSlider = new Slider() { Value = 1 };
            PageLastIndexLable = new Label() { Text = "1" };

            DocTreeView = new ListView();
            PageScrollViewer = new ScrollView() { };
            PageImage = new Image() { };

            WindowPage.AddElement(SelectFileButton);
            WindowPage.AddElement(ShowPdfButton);
            WindowPage.AddElement(PageScaleLable);
            WindowPage.AddElement(PageScaleTextBox);
            WindowPage.AddElement(PageFirstIndexLable);
            WindowPage.AddElement(PageIndexSlider);
            WindowPage.AddElement(PageLastIndexLable);
            WindowPage.AddElement(DocTreeView);
            WindowPage.AddElement(PageScrollViewer);

            using (var set = new FluentConstraintSet())
            {
                set.Clone(WindowPage);
                if (System.OperatingSystem.IsWindows())
                {
                    set.Select(SelectFileButton).LeftToLeft(null, 20).TopToTop(null, 10)
                        .Select(ShowPdfButton).LeftToRight(SelectFileButton, 20).TopToTop(null, 10)
                        .Select(PageScaleLable).LeftToRight(ShowPdfButton, 20).CenterYTo(ShowPdfButton)
                        .Select(PageScaleTextBox).LeftToRight(PageScaleLable, 20).CenterYTo(ShowPdfButton)
                        .Select(PageFirstIndexLable).LeftToRight(PageScaleTextBox, 20).CenterYTo(ShowPdfButton)
                        .Select(PageIndexSlider).LeftToRight(PageFirstIndexLable, 20).CenterYTo(ShowPdfButton).RightToLeft(PageLastIndexLable, 20).Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageLastIndexLable).LeftToRight(PageIndexSlider, 20).RightToRight(null, 20).CenterYTo(ShowPdfButton)
                        //.Select(DocTreeView).LeftToLeft(SelectFileButton).TopToBottom(SelectFileButton, 5).BottomToBottom(null, 5).Width(200).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        //.Select(PageScrollViewer).LeftToRight(DocTreeView, 20).RightToRight(null, 20).TopToBottom(ShowPdfButton, 5).BottomToBottom(null, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint).Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        ;
                }
                else
                {
                    set.Select(SelectFileButton).LeftToLeft(null, 20).TopToTop(null, 20)
                        .Select(ShowPdfButton).LeftToRight(SelectFileButton, 20).TopToTop(SelectFileButton)
                        .Select(PageScaleLable).LeftToLeft(ShowPdfButton, 20).CenterYTo(ShowPdfButton)
                        .Select(PageScaleTextBox).LeftToRight(PageScaleLable, 20).CenterYTo(ShowPdfButton)
                        .Select(PageFirstIndexLable).LeftToLeft(SelectFileButton, 20).TopToBottom(SelectFileButton, 20)
                        .Select(PageIndexSlider).LeftToRight(PageFirstIndexLable, 20).CenterYTo(PageFirstIndexLable).RightToLeft(PageLastIndexLable, 20).Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                        .Select(PageLastIndexLable).LeftToRight(PageIndexSlider, 20).RightToRight(null, 20).CenterYTo(PageFirstIndexLable)
                        .Select(DocTreeView).LeftToLeft(SelectFileButton).TopToBottom(PageFirstIndexLable, 5)
                        //.BottomToBottom(null, 5)
                        .Width(200).Height(FluentConstraintSet.SizeBehavier.WrapContent)
                        .Select(PageScrollViewer).LeftToLeft(null, 20).RightToRight(null, 20).TopToBottom(PageFirstIndexLable, 5).BottomToBottom(null, 5).Width(FluentConstraintSet.SizeBehavier.MatchConstraint).Height(FluentConstraintSet.SizeBehavier.MatchConstraint);

                }
                set.ApplyTo(WindowPage);
            }

            PageScrollViewer.Content = PageImage;

            SelectFileButton.Clicked += SelectFileButton_ClickedAsync;
            ShowPdfButton.Clicked += ShowPdfButton_Clicked;
            PageIndexSlider.ValueChanged += (sender, e) =>
            {
                ShowPdfButton_Clicked(null, null);
            };
            init();
            ReadPDFAsyncFormResourcesAsync();
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
        void init()
        {
            var library = Pdfium.Instance;
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