using ApplePDF.PdfKit;
using Xunit;
#if IOS || MACCATALYST
using Lib = ApplePDF.PdfKit.PdfKitLib;
#else
using Lib = ApplePDF.PdfKit.PdfiumLib;
#endif

namespace ApplePDF.ApplePdfKit.Test.Tests
{
    public class PdfDocumentTests
    {
        private readonly ILib _fixture = Lib.Instance;
        PdfDocument LoadPdfDocument(string path, string password)
        {
            //原资源名 Docs/Docnet/simple_0.pdf
            //嵌入后的资源名 ApplePDF.ApplePdfKit.Test.Docnet.simple_0.pdf
            if (path.Contains("Docs/"))
            {
                path = path.Replace("Docs/", "");
            }
            path = path.Replace('/', '.');
            var fileName = "ApplePDF.ApplePdfKit.Test." + path;
            return _fixture.LoadPdfDocument(ReadEmbedAssetBytes(fileName), password);
        }

        public byte[] ReadEmbedAssetBytes(string resourcePath = "foler.fileName.extention")
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(resourcePath))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }


        public PdfDocumentTests()
        {

        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 19)]
        [InlineData("Docs/Docnet/simple_1.pdf", null, 5)]
        [InlineData("Docs/Docnet/simple_2.pdf", null, 10)]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 2)]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 3)]
        public void PageCount_WhenCalled_ShouldReturnCorrectResults(string filePath, string password, int expectedCount)
        {
            using (var doc = LoadPdfDocument(filePath, password))
            {
                var count = doc.PageCount;
                Assert.Equal(expectedCount, count);
            }
        }

        [Fact]
        public void PageCount_WhenCalledWhenDisposed_ShouldThrow()
        {
            var doc = LoadPdfDocument("Docs/Docnet/simple_0.pdf", null);
            doc.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var c = doc.PageCount;
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 1, 7)]
        [InlineData("Docs/Docnet/simple_1.pdf", null, 1, 3)]
        [InlineData("Docs/Docnet/simple_2.pdf", null, 1, 2)]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1, 3)]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 1, 7)]
        public void MajorAndMinorVersionTest(string filePath, string password, int expectedMajorVersion, int expectedMinorVersion)
        {
            using (var doc = LoadPdfDocument(filePath, password))
            {
                Assert.Equal(expectedMajorVersion, doc.MajorVersion);
                Assert.Equal(expectedMinorVersion, doc.MinorVersion);
            }
        }

        [Theory]
        [InlineData("Docs/Docnet/protected_0.pdf", null, -3)]
        [InlineData("Docs/Docnet/protected_0.pdf", null, 10)]
        public void GetPage_WhenCalledWithInvalidFile_ShouldThrow(string filePath, string password, int pageIndex)
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                using (var doc = LoadPdfDocument(filePath, password))
                using (var page = doc.GetPage(pageIndex))
                {
                }
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf", null, -1)]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 19)]
        public void GetPage_WhenCalledWithInvalidIndex_ShouldThrow(string filePath, string password, int pageIndex)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var doc = LoadPdfDocument(filePath, password))
                using (var page = doc.GetPage(pageIndex))
                {
                }
            });
        }

        [Fact]
        public void GetPage_WhenCalledWhenDisposed_ShouldThrow()
        {
            var doc = LoadPdfDocument("Docs/Docnet/simple_0.pdf", null);
            doc.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                doc.GetPage(0);
            });
        }

        [Theory]
        [InlineData("Docs/mytest_VulkanGuideline.pdf")]
        public void OutlineRoot_WhenCall_GetAllOutlineOfPdf(string filePath)
        {
            using (var doc = LoadPdfDocument(filePath, null))
            {
                var outline = doc.OutlineRoot;
                Assert.True(outline.Children[1].Children.Count > 0);
            }
        }

        [Theory]
        [InlineData("Docs/mytest_VulkanGuideline.pdf")]
        public void CreatePage_AsLastPage_Test(string filePath)
        {
            byte[] data;
            int prePageCount = 0;
            //测试新页面是否增加
            using (var doc = LoadPdfDocument(filePath, null) as IPdfDocument)
            {
                prePageCount = doc.PageCount;
                var size = doc.GetPage(0).GetSize();
                doc.CreatePage(prePageCount, (int)size.Width, (int)size.Height);
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount + 1, nowPageCount);
                data = _fixture.Save(doc as PdfDocument);
            }
            //测试新页面是否保存
            using (var doc = _fixture.LoadPdfDocument(data, null) as IPdfDocument)
            {
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount + 1, nowPageCount);
            }
        }

        [Theory]
        [InlineData("Docs/mytest_VulkanGuideline.pdf")]
        public void CreatePage_Insert_Test(string filePath)
        {
            byte[] data;
            int prePageCount = 0;
            //测试新页面是否增加
            using (var doc = LoadPdfDocument(filePath, null) as IPdfDocument)
            {
                prePageCount = doc.PageCount;
                var size = doc.GetPage(0).GetSize();
                doc.CreatePage(10, (int)size.Width, (int)size.Height);
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount + 1, nowPageCount);
                data = _fixture.Save(doc as PdfDocument);
            }
            //测试新页面是否保存
            using (var doc = _fixture.LoadPdfDocument(data, null) as IPdfDocument)
            {
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount + 1, nowPageCount);
            }
        }
        
        [Theory]
        [InlineData("Docs/mytest_VulkanGuideline.pdf")]
        public void RemovePageTest(string filePath)
        {
            byte[] data;
            int prePageCount = 0;
            //测试新页面是否增加
            using (var doc = LoadPdfDocument(filePath, null) as IPdfDocument)
            {
                prePageCount = doc.PageCount;
                doc.RemovePage(1);
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount -1, nowPageCount);
                data = _fixture.Save(doc as PdfDocument);
            }
            //测试新页面是否保存
            using (var doc = _fixture.LoadPdfDocument(data, null) as IPdfDocument)
            {
                var nowPageCount = doc.PageCount;
                Assert.Equal(prePageCount - 1, nowPageCount);
            }
        }
    }
}
