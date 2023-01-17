using ApplePDF.PdfKit;
using ObjCRuntime;
using System.Drawing;

namespace ApplePDF.Demo.Mac
{
    public partial class ViewController : NSViewController
    {
        protected ViewController(NativeHandle handle) : base(handle)
        {
            // This constructor is required if the view controller is loaded from a xib or a storyboard.
            // Do not put any initialization here, use ViewDidLoad instead.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            var view = new NSImageView();
            view.ImageAlignment = NSImageAlignment.Top;
            view.ImageScaling = NSImageScale.AxesIndependently;
            var data = AssetsManager.ReadEmbedAssetBytes("pdfBible.pdf");
            var doc = new ApplePDF.PdfKit.PdfDocument(data, null);
            var page = doc.GetPage(0);
            var rect = page.GetBoundsForBox(PdfDisplayBox.Media);
            var size = new SizeF(rect.Width, rect.Height);
            view.Image = new NSImage(page.Draw((int)size.Width, (int)size.Height), new CGSize(size.Width, size.Height));
            this.View = view;
        }

        public override NSObject RepresentedObject
        {
            get => base.RepresentedObject;
            set
            {
                base.RepresentedObject = value;

                // Update the view, if already loaded.
            }
        }
    }
}