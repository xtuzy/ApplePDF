#if __ANDROID__
using Android.App;
#endif
using NUnit.Framework;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Net.Test
{


    [TestFixture]
    public class PdfiumCoreTests
    {
        Stream ReadPdf(string filePath)
        {
#if __ANDROID__
            return  Xamarin.Helper.Files.FileHelper.ReadMemoryStreamFromAssets(Android.Test.MainActivity.Instance, "XamarinBinding.pdf");
#else
            return File.OpenRead(filePath);
#endif
        }

        [TestCase("Docs/mytest_10_pagecount.pdf",10)]
        public void PdfiumCore_WhenCalled_ShouldGetCorrectPageCount(string filePath,int pageCount)
        {
            using (var stream = ReadPdf(filePath))
            {
                fpdfview.FPDF_InitLibrary();
                //https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/PdfFile.cs
                var _id = StreamManager.Register(stream);
                var doc = fpdfview.FPDF_LoadDocument(stream, null, _id);
                var count = fpdfview.FPDF_GetPageCount(doc);
                fpdfview.FPDF_CloseDocument(doc);
                fpdfview.FPDF_DestroyLibrary();
                Assert.AreEqual(pageCount, count);
            }  
        }
    }
}
