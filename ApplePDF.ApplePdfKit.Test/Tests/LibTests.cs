using System;
using System.IO;
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
        private readonly ILib _fixture =  Lib.Instance;

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
    }
}