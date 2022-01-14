using NUnit.Framework;
using PDFiumCore;

namespace Pdf.Net.Desktop.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void TestPdfiumBinding()
        {
            //var stream = Xamarin.Helper.Files.FileHelper.ReadMemoryStreamFromAssets((Activity)Application.Context, "XamarinBinding.pdf");
            //TestContext.WriteLine(stream.Length);
            fpdfview.FPDF_InitLibrary();
            //https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfFile.cs
            //var _id = StreamManager.Register(stream);
            var doc = fpdfview.FPDF_LoadDocument("XamarinBinding.pdf",null);
            var count = fpdfview.FPDF_GetPageCount(doc);
            Assert.IsTrue(count > 0);
        }
    }
}