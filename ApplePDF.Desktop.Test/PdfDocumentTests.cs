using System;
using System.IO;
using NUnit.Framework;
using ApplePDF.PdfKit;

namespace ApplePDF.Test
{
    [TestFixture]
    internal class PdfDocumentTests
    {
        private readonly Pdfium _fixture = Pdfium.Instance;

        public PdfDocumentTests()
        {
        
        }

        [TestCase("Docs/Docnet/simple_0.pdf", null, 19)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 19)]
        [TestCase("Docs/Docnet/simple_1.pdf", null, 5)]
        [TestCase("Docs/Docnet/simple_1.pdf", null, 5)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 10)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 10)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 2)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 2)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 3)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 3)]
        public void PageCount_WhenCalled_ShouldReturnCorrectResults(string filePath, string password, int expectedCount)
        {
            using (var doc = _fixture.LoadPdfDocument(filePath, password))
            {
                var count = doc.PageCount;

                Assert.AreEqual(expectedCount, count);
            }
        }

        [Test]
        public void PageCount_WhenCalledWhenDisposed_ShouldThrow()
        {
            var doc = _fixture.LoadPdfDocument("Docs/Docnet/simple_0.pdf", null);
            doc.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var c = doc.PageCount;
            });
        }

        [TestCase("Docs/Docnet/simple_0.pdf", null, 17)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 17)]
        [TestCase("Docs/Docnet/simple_1.pdf", null, 13)]
        [TestCase("Docs/Docnet/simple_1.pdf", null, 13)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 12)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 12)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 13)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 13)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 17)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 17)]
        public void MajorVersion_WhenCalled_ShouldReturnCorrectResults(string filePath, string password, int expectedVersion)
        {
            using (var doc = _fixture.LoadPdfDocument(filePath, password))
            {
                var version = doc.MajorVersion;

                Assert.AreEqual(expectedVersion, version);
            }
        }

        [Test]
        public void MajorVersion_WhenCalledWhenDisposed_ShouldThrow()
        {
            var doc = _fixture.LoadPdfDocument("Docs/Docnet/simple_0.pdf", null);
            doc.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var v = doc.MajorVersion;
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/protected_0.pdf", null, -3)]
        [TestCase("Docs/Docnet/protected_0.pdf", null, 10)]
        [TestCase("Docs/Docnet/protected_0.pdf", null, -3)]
        [TestCase("Docs/Docnet/protected_0.pdf", null, 10)]
        public void GetPage_WhenCalledWithInvalidFile_ShouldThrow(string filePath, string password, int pageIndex)
        {
            Assert.Throws<Exception>(() =>  {
                using (var doc = _fixture.LoadPdfDocument(filePath, password))
                using (var page = doc.GetPage(pageIndex))
                {
                }
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_0.pdf", null, -1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 19)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, -1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 19)]
        public void GetPage_WhenCalledWithInvalidIndex_ShouldThrow(string filePath, string password, int pageIndex)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                using (var doc = _fixture.LoadPdfDocument(filePath, password))
                using (var page = doc.GetPage(pageIndex))
                {
                }
            });
        }

        [Test]
        public void GetPage_WhenCalledWhenDisposed_ShouldThrow()
        {
            var doc = _fixture.LoadPdfDocument("Docs/Docnet/simple_0.pdf", null);
            doc.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                doc.GetPage(0);
            });
        }

    }
}
