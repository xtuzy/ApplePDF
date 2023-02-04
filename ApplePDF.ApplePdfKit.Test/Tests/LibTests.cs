using ApplePDF.PdfKit;
using Xunit;
#if IOS || MACCATALYST
using Lib = ApplePDF.PdfKit.PdfKitLib;
#else
using Lib = ApplePDF.PdfKit.PdfiumLib;
#endif
namespace ApplePDF.Test
{
    public sealed class LibTests
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

        public LibTests()
        {

        }

        [Fact]
        public void LoadPdfDocument_WhenCalledWithNullFilePath_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _fixture.LoadPdfDocument((string)null, null));
        }

        [Fact]
        public void LoadPdfDocumnet_WhenCalledWithNullStream_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _fixture.LoadPdfDocument((Stream)null, null));
        }

        [Fact]
        public void LoadPdfDocument_WhenCalledWithNoPassword_ShouldThrow()
        {
            Assert.ThrowsAny<Exception>(() => _fixture.LoadPdfDocument("Docs/Docnet/protected_0.pdf", null));
        }

        [Fact]
        public void CreatePdfDocumentTest()
        {
            byte[] data;
            using (var doc = _fixture.CreatePdfDocument() as IPdfDocument)
            {
                Assert.Equal(0, doc.PageCount);
                doc.CreatePage(0, 500, 800);
                Assert.Equal(1, doc.PageCount);
                data = _fixture.Save(doc as PdfDocument);
            }
            using (var doc = _fixture.LoadPdfDocument(data, null) as IPdfDocument)
            {
                Assert.Equal(1, doc.PageCount);
            }
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf", "Docs/Docnet/simple_1.pdf")]
        public void MergeTest(string firstPath, string secondPath)
        {
            using var doc1 = LoadPdfDocument(firstPath, null);
            var doc1Count = doc1.PageCount;
            using var doc2 = LoadPdfDocument(secondPath, null);
            var doc2Count = doc2.PageCount;
#if IOS || MACCATALYST
            var pdfkit = (_fixture as PdfKit.PdfKitLib);
            var data = pdfkit.Merge(doc1, doc2);
            // 在mac上检验
            var path = Path.Combine(new DirectoryInfo(System.Environment.CurrentDirectory).Parent.FullName, "result.pdf");
            if (!File.Exists(path))
                File.Create(path);
            if (File.Exists(path))
            {
                using var stream = new FileStream(path, FileMode.Open);
                data.AsStream().CopyTo(stream);
            }
            var mergeDoc = pdfkit.LoadPdfDocument(data);
            Assert.Equal(doc1Count + doc2Count, mergeDoc.PageCount);
#else
            var pdfium = (_fixture as PdfKit.PdfiumLib);
            var mergeDoc = pdfium.Merge(doc1, doc2);
            Assert.Equal(doc1Count + doc2Count, mergeDoc.PageCount);
#endif
        }

        [Theory]
        [InlineData("Docs/mytest_VulkanGuideline.pdf")]
        public void SplitTest(string firstPath)
        {
            var doc1 = LoadPdfDocument(firstPath, null);
            int fromIndex = 5;
            int toIndex = 10;
#if IOS || MACCATALYST
            var pdfkit = (_fixture as PdfKit.PdfKitLib);
            var mergeDoc = pdfkit.LoadPdfDocument(pdfkit.Split(doc1, fromIndex, toIndex));
            Assert.Equal(toIndex - fromIndex + 1, mergeDoc.PageCount);
#else
            var pdfium = (_fixture as PdfKit.PdfiumLib);
            var mergeDoc = pdfium.Split(doc1, fromIndex, toIndex);
            Assert.Equal(toIndex - fromIndex + 1, mergeDoc.PageCount);
#endif
        }
    }
}