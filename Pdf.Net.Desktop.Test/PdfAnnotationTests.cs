using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdf.Net.PdfKit;
using Pdf.Net.PdfKit.Annotation;

namespace Pdf.Net.Test
{
    [TestFixture]
    public class PdfAnnotationTests
    {
        private readonly Pdfium _fixture = Pdfium.Instance;

        private void ExecuteForDocument(string filePath, string password, int pageIndex, Action<PdfPage> action)
        {
            using (var doc = _fixture.LoadPdfDocument(filePath, password))
            using (var page = doc.GetPage(pageIndex))
            {
                action(page);
            }
        }


        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_lineannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_textannotation.pdf", PdfAnnotationSubtype.FreeText)]
        public void Type_WhenCalled_ShouldGetCorrectAnnotationType(string filePath,PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.AreEqual(type, annot.AnnotationType);
            });
        }
    }
}
