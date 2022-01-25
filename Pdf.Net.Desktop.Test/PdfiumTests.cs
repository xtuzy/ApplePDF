using System;
using System.IO;
using NUnit.Framework;
using Pdf.Net.PdfKit;
namespace Pdf.Net.Test
{
    [TestFixture]
    public sealed class PdfiumTests
    {
        private readonly Pdfium _fixture =   Pdfium.Instance;

        public PdfiumTests()
        {
        
        }

        [Test]
        public void LoadPdfDocument_WhenCalledWithNullFilePath_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _fixture.LoadPdfDocument((string)null, null));
        }

        [Test]
        public void LoadPdfDocumnet_WhenCalledWithNullStream_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _fixture.LoadPdfDocument((Stream)null, null));
        }

        [Test]
        public void LoadPdfDocument_WhenCalledWithNoPassword_ShouldThrow()
        {
            Assert.Throws<Exception>(() => _fixture.LoadPdfDocument("Docs/protected_0.pdf", null));
        }

        
       
    }
}