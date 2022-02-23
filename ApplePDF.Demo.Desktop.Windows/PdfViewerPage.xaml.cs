using ApplePDF.Demo.Desktop.Windows.Extension;
using ApplePDF.PdfKit;
using PDFiumCore;
using SharpConstraintLayout.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Essentials;

namespace ApplePDF.Demo.Desktop.Windows
{
    /// <summary>
    /// Interaction logic for PdfViewerPage.xaml
    /// </summary>
    public partial class PdfViewerPage : Page
    {
        private PdfDocument doc;
        private ConstraintLayout WindowPage;
        private Button SelectFileButton;
        private Button ShowPdfButton;
        private Label PageFirstIndexLable;
        private Slider PageIndexSlider;
        private Label PageLastIndexLable;
        private Label PageScaleLable;
        private TextBox PageScaleTextBox;
        private TreeView DocTreeView;
        private ScrollViewer PageScrollViewer;
        private System.Windows.Controls.Image PageImage;
        string defaultDoc = "pdfBible.pdf";
        public PdfViewerPage()
        {
            InitializeComponent();

            WindowPage = new ConstraintLayout() { Background = new SolidColorBrush(Colors.DarkGray) };
            Content = WindowPage;

            SelectFileButton = new Button() { Content = "选择" };
            ShowPdfButton = new Button() { Content = "打开" };
            PageScaleLable = new Label() { Content = "放大倍数" };
            PageScaleTextBox = new TextBox() { Text = "1" };
            PageFirstIndexLable = new Label() { Content = "1" };
            PageIndexSlider = new Slider() { Value = 1 };
            PageLastIndexLable = new Label() {Content="1" };
            
            DocTreeView = new TreeView();
            PageScrollViewer = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
            PageImage = new System.Windows.Controls.Image() { Stretch = Stretch.None };

            WindowPage.Children.Add(SelectFileButton);
            WindowPage.Children.Add(ShowPdfButton);
            WindowPage.Children.Add(PageFirstIndexLable);
            WindowPage.Children.Add(PageIndexSlider);
            WindowPage.Children.Add(PageLastIndexLable);
            WindowPage.Children.Add(PageScaleLable);
            WindowPage.Children.Add(PageScaleTextBox);
            WindowPage.Children.Add(DocTreeView);
            WindowPage.Children.Add(PageScrollViewer);

            SelectFileButton.LeftToLeft(WindowPage, 20).TopToTop(WindowPage, 20);
            ShowPdfButton.LeftToRight(SelectFileButton, 20).TopToTop(SelectFileButton);
            PageScaleLable.LeftToRight(ShowPdfButton, 20).BaselineToBaseline(ShowPdfButton);
            PageScaleTextBox.LeftToRight(PageScaleLable, 20).BaselineToBaseline(ShowPdfButton);
            PageFirstIndexLable.LeftToRight(PageScaleTextBox, 20).BaselineToBaseline(ShowPdfButton);
            PageIndexSlider.LeftToRight(PageFirstIndexLable, 20).CenterYTo(ShowPdfButton)
                .RightToLeft(PageLastIndexLable).WidthEqualTo(ConstraintSet.SizeType.MatchConstraint);
            PageLastIndexLable.LeftToRight(PageIndexSlider, 20).RightToRight(WindowPage,20).BaselineToBaseline(ShowPdfButton);
            
            DocTreeView
                .LeftToLeft(SelectFileButton)
                .TopToBottom(SelectFileButton, 5)
                .BottomToBottom(WindowPage,5)
                .WidthEqualTo(200)
                .HeightEqualTo(ConstraintSet.SizeType.MatchConstraint);
            PageScrollViewer
                .LeftToRight(DocTreeView, 20)
                .RightToRight(WindowPage, 20)
                .TopToBottom(ShowPdfButton, 5)
                .BottomToBottom(WindowPage, 5)
                .WidthEqualTo(ConstraintSet.SizeType.MatchConstraint)
                .HeightEqualTo(ConstraintSet.SizeType.MatchConstraint);

            PageScrollViewer.Content = PageImage;

            SelectFileButton.Click += SelectFileButton_ClickAsync;
            ShowPdfButton.Click += ShowPdfButton_Click;
            PageIndexSlider.ValueChanged += ShowPdfButton_Click;

            ReadPDF(defaultDoc);

        }

        void ReadPDF(string filePath)
        {
            this.WindowTitle = filePath;
            var stream = File.OpenRead(filePath);
            if (doc != null)
                doc.Dispose();
            doc = Pdfium.Instance.LoadPdfDocument(stream, null);
            PageLastIndexLable.Content = $"{doc.PageCount}";
            PageIndexSlider.Maximum = doc.PageCount;
            PageIndexSlider.InvalidateVisual();
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

        private async void SelectFileButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var result = await Xamarin.Essentials.FilePicker.PickAsync(Xamarin.Essentials.PickOptions.Default);
            if (result == null)
                return;
            ReadPDF(result.FullPath);
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        private void ShowPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)PageIndexSlider.Value-1;
            if(index < 0) index=0;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr==String.Empty?"1":scaleStr);
           
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            //using (var img = PdfPageExtension.RenderPageToPdfNativeBitmap(doc.GetPage(index), scale, flags))
            using (var img = PdfPageExtension.RenderPageToBitmap(doc.GetPage(index), scale, flags))
            //using (var img = PdfPageExtension.RenderPageBySKBitmap(doc.GetPage(index), scale, flags))
            {
                Debug.WriteLine($"Bitmap({img.Width},{img.Height})");
                var bitmapImage = BitmapToBitmapImage(img);
                Debug.WriteLine($"BitmapImage(Dp:{bitmapImage.Width},{bitmapImage.Height};Pixel:{bitmapImage.PixelWidth},{bitmapImage.PixelHeight};{bitmapImage.DpiX})");
                PageImage.Source = bitmapImage;
            }
            WindowPage.UpdateLayout();
        }
    }
}
