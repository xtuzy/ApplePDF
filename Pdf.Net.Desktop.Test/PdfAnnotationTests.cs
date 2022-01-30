using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        public void Type_WhenCalled_ShouldGetCorrectAnnotationType(string filePath,PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.AreEqual(type, annot.AnnotationType);
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        public void Color_WhenCalled_ShouldGetAnnotationColor(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
               // Assert.Positive(annot.Color.A);
                Assert.Positive(annot.Color.R);
                Assert.Positive(annot.Color.G);
                Assert.Positive(annot.Color.B);
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        public void Position_WhenCalled_ShouldGetAnnotationPosion(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.Positive(annot.Position.Width);
                Assert.Positive(annot.Position.Height);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Link)]
        public void AddAnnotation_WhenCalledWithHighlightAnnotation_ShouldShowRectangleAnnotation(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0,annots.Count);
                var annot = new PdfHighlightAnnotation(PdfAnnotationSubtype.Highlight);
                var bounds = pageReader.GetBoundsForBox();
                var annotSize = new SizeF(100, 30);
                annot.Position = new RectangleF(bounds.Width/2-annotSize.Width/2, bounds.Height-10-annotSize.Height/2, annotSize.Width, annotSize.Height);
                annot.Color = Color.Cyan;
                pageReader.AddAnnotation(annot);
                annot.AppendAnnotationPoint(annot.Position);
                annot.Dispose();
                Pdfium.Instance.Save(pageReader.Document, "Result.pdf");
                Debug.WriteLine(pageReader.AnnotationCount);
                Assert.Less(0,pageReader.AnnotationCount);
            });
        }
    }
}
