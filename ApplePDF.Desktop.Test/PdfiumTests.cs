using System;
using System.IO;
using NUnit.Framework;
using ApplePDF.PdfKit;
namespace ApplePDF.Test
{
    [TestFixture]
    public sealed class PdfiumTests
    {
        private readonly PdfiumLib _fixture =   PdfiumLib.Instance;

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
            Assert.Throws<Exception>(() => _fixture.LoadPdfDocument("Docs/Docnet/protected_0.pdf", null));
        }
    }
}