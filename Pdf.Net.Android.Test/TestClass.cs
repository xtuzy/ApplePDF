using Android.App;
using NUnit.Framework;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Net.Android.Test
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }

        [Test]
        public void TestPdfiumBinding()
        {
            var stream = Xamarin.Helper.Files.FileHelper.ReadMemoryStreamFromAssets(MainActivity.Instance,"XamarinBinding.pdf");
            TestContext.WriteLine(stream.Length);
            fpdfview.FPDF_InitLibrary();
            //https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfFile.cs
            var _id = StreamManager.Register(stream);
            //var doc = Docnet.Core.Bindings.fpdf_view.FPDF_LoadCustomDocument(stream, null,_id);
            var doc = fpdfview.FPDF_LoadDocument(stream, null,_id);

            var count = fpdfview.FPDF_GetPageCount(doc);
            Assert.IsTrue(count > 0);
        }
    }
}
