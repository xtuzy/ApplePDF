﻿using Pdf.Net.Desktop.Windows.Extension;
using Pdf.Net.PdfKit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xamarin.Essentials;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using SharpConstraintLayout.Wpf;

namespace Pdf.Net.Desktop.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PdfDocument doc;
        private ConstraintLayout WindowPage;
        private Button ShowPdfButton;
        private Label PageIndexLable;
        private TextBox PageIndexTextBox;
        private Label PageCountLable;
        private ScrollViewer PageScrollViewer;
        private System.Windows.Controls.Image PageImage;

        public MainWindow()
        {
            InitializeComponent();
            doc = Pdfium.Instance.LoadPdfDocument("XamarinBinding.pdf", null);
            
            WindowPage = new ConstraintLayout() { Background = new SolidColorBrush(Colors.DarkGray) };
            Content = WindowPage;

            ShowPdfButton = new Button() { Content = "打开" };
            PageIndexLable = new Label() { Content = "页码" };
            PageIndexTextBox = new TextBox() { Text = "1" };
            PageCountLable = new Label() { Content = $"总页数:{doc.PageCount}",Background= new SolidColorBrush(Colors.AliceBlue) };
            PageScrollViewer = new ScrollViewer() { Background=new SolidColorBrush(Colors.Blue),HorizontalScrollBarVisibility=ScrollBarVisibility.Auto};
            PageImage = new System.Windows.Controls.Image() { Stretch = Stretch.None};

            WindowPage.Children.Add(ShowPdfButton);
            WindowPage.Children.Add(PageIndexLable);
            WindowPage.Children.Add(PageIndexTextBox);
            WindowPage.Children.Add(PageCountLable);
            WindowPage.Children.Add(PageScrollViewer);

            ShowPdfButton.LeftToLeft(WindowPage, 20).TopToTop(WindowPage, 20);
            PageIndexLable.LeftToRight(ShowPdfButton).BaselineToBaseline(ShowPdfButton);
            PageIndexTextBox.LeftToRight(PageIndexLable).BaselineToBaseline(PageIndexLable).WidthEqualTo(50);
            PageCountLable.LeftToRight(PageIndexTextBox,20).BaselineToBaseline(PageIndexTextBox).WidthEqualTo(100);
            PageScrollViewer.LeftToLeft(WindowPage, 20).RightToRight(WindowPage, 20).TopToBottom(ShowPdfButton, 20).BottomToBottom(WindowPage, 20).HeightEqualTo(ConstraintSet.SizeType.MatchConstraint);

            PageScrollViewer.Content = PageImage;

            ShowPdfButton.Click += ShowPdfButton_Click;
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
            var index = int.Parse(PageIndexTextBox.Text);
            var density = DeviceDisplay.MainDisplayInfo.Density;

            //var img = RenderPageExtension.RenderPageToPdfNativeBitmap(doc,index, (float)DeviceDisplay.MainDisplayInfo.Density*2f);
            //var img = RenderPageExtension.RenderPageToBitmap(doc, index, (float)DeviceDisplay.MainDisplayInfo.Density*2);
            var img = PdfPageExtension.RenderPageBySKBitmap(doc.GetPage(index), (float)DeviceDisplay.MainDisplayInfo.Density*2f);
            Debug.WriteLine($"Bitmap({img.Width},{img.Height})");
            var bitmapImage = BitmapToBitmapImage(img);
            Debug.WriteLine($"BitmapImage(Dp:{bitmapImage.Width},{bitmapImage.Height};Pixel:{bitmapImage.PixelWidth},{bitmapImage.PixelHeight};{bitmapImage.DpiX})");
            PageImage.Source = bitmapImage;
        }
    }
}