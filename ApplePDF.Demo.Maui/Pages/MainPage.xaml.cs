using ApplePDF.Demo.Maui.Extension;
using ApplePDF.Demo.Maui.Helper;
using ApplePDF.Demo.Maui.Services;
using ApplePDF.PdfKit;
using CommunityToolkit.Mvvm.ComponentModel;
using SharpConstraintLayout.Maui.Widget;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
#if IOS || MACCATALYST
using Lib = ApplePDF.PdfKit.PdfKitLib;
#else
using Lib = ApplePDF.PdfKit.PdfiumLib;
using PDFiumCore;
#endif
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
                    ShowPdfAsync();
                }
            };
            SelectPdfFormResourcesAsync();
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

        private async void ShowPdfAsync()
        {
            var index = int.Parse(PageCurrentIndexEntry.Text) - 1;
            if (index < 0) index = 0;

            var scaleStr = PageScaleTextBox.Text;
            var scale = float.Parse(scaleStr == String.Empty ? "1" : scaleStr);
#if ANDROID
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
#elif WINDOWS
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var flags = (int)(RenderFlags.OptimizeTextForLcd | RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting);
            using var page = doc.GetPage(index);
            MemoryStream stream = null;
            var imageBuffer = page.Draw(scale, scale, PdfRotate.Degree0, flags);
            int width = (int)(page.GetSize().Width * scale);
            int height = (int)(page.GetSize().Height * scale);
            Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap myWriteableBitmap = null;
            this.Dispatcher.Dispatch(async () =>
            {
                myWriteableBitmap = new Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap(width, height);
                using Stream pixelStream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(myWriteableBitmap.PixelBuffer);
                await pixelStream.WriteAsync(imageBuffer, 0, imageBuffer.Length);
                myWriteableBitmap.Invalidate();
                PageImage.Source = new PlatformImageImageSource() { PlatformImage = myWriteableBitmap };
                WindowPage.RequestReLayout();
            });
            
#elif IOS || MACCATALYST
            using var page = doc.GetPage(index);
            var r = page.Page.Rotation;
            var size = page.GetSize();
            var uiimage = page.Draw(1);
            if (this.Dispatcher.IsDispatchRequired)
                this.Dispatcher.Dispatch(() =>
                {
                    PageImage.Source = null;
                    PageImage.Source = new PlatformImageImageSource() { PlatformImage = uiimage };
                    WindowPage.RequestReLayout();
                });
            else
            {
                PageImage.Source = null;
                PageImage.Source = new PlatformImageImageSource() { PlatformImage = uiimage};
                WindowPage.RequestReLayout();
            }

#endif
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
            ReadPdfAsync(result.FullPath);
            ReadBookmark(doc);
            ShowPdfAsync();
        }

        async Task SelectPdfFormResourcesAsync(string filePath = null)
        {
            if (filePath == null)
            {
                await FileSystem.OpenAppPackageFileAsync(defaultDoc).ContinueWith(t =>
                {
                    MemoryStream memoryStream = new MemoryStream();
                    t.Result.CopyTo(memoryStream);
#if IOS || MACCATALYST
                    ReadPdfAsync(memoryStream.ToArray());
#else
                    ReadPdfAsync(memoryStream);
#endif
                    ReadBookmark(doc);
                    ShowPdfAsync();
                });
            }
        }

        void ReadPdfAsync(string path)
        {
            if (doc != null)
                doc.Dispose();
            doc = Lib.Instance.LoadPdfDocument(path, null);
        }

        void ReadPdfAsync(byte[] data)
        {
            if (doc != null)
                doc.Dispose();
            doc = Lib.Instance.LoadPdfDocument(data, null);
        }

        void ReadPdfAsync(Stream stream)
        {
            if (doc != null)
                doc.Dispose();
            doc = Lib.Instance.LoadPdfDocument(stream, null);
        }

        void ReadBookmark(PdfDocument doc) 
        { 
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