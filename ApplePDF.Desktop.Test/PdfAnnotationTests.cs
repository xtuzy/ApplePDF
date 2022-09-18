using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using NUnit.Framework;
using System;
using System.Drawing;

namespace ApplePDF.Test
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
        public void Type_WhenCalled_ShouldGetCorrectAnnotationType(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.AreEqual(type, annot.AnnotationType);
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "Yellow")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "Red")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "Transparent")]
        public void AnnotColor_WhenCalled_ShouldNotAllCasePass(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                var expectedColor = System.Drawing.Color.FromName(color);
                if (annot.AnnotColor != null)
                    Assert.AreEqual(expectedColor, annot.AnnotColor.Value);
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "Yellow")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "Red")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "Transparent")]
        public void FillColor_WhenCalled_ShouldGetCurrentColor(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                if ((annot as PdfFreeTextAnnotation)?.FillColor != null)
                    Assert.AreEqual(System.Drawing.Color.FromName(color), (annot as PdfFreeTextAnnotation).FillColor.Value);
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "Yellow")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "Blue")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "Red")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "Transparent")]
        public void StrokeColor_WhenCalled_ShouldGetCurrentColor(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                if ((annot as PdfFreeTextAnnotation)?.StrokeColor != null)
                    Assert.AreEqual(System.Drawing.Color.FromName(color), (annot as PdfFreeTextAnnotation).StrokeColor.Value);
                else
                    Assert.Ignore();
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
                Assert.Positive(annot.AnnotBox.Width);
                Assert.Positive(annot.AnnotBox.Height);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Highlight)]
        public void AddAnnotation_WhenCalledWithHighlightAnnotation_ShouldShowRectangleAnnotation(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfHighlightAnnotation(type);
                var pageBounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                annot.AnnotBox = new RectangleF(pageBounds.Width / 2 - annotSize.Width / 2, pageBounds.Height / 2 - annotSize.Height / 2, annotSize.Width, annotSize.Height);
                //annot.Color = Color.Cyan;
                pageReader.AddAnnotation(annot);
                //annot.AppendAnnotationPoint(annot.AnnotBox);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(1, annots.Count);
                Assert.AreEqual(PdfAnnotationSubtype.Highlight, annots[0].AnnotationType);
            });
        }

        #region FreeText

        [TestCase("Docs/mytest_4_freetextannotation.pdf")]
        public void GetText_WhenCalled_ShouldGetText(string filePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(4, annots.Count);
                Assert.IsTrue((annots[0] as PdfFreeTextAnnotation).Text.Contains("1"));
                Assert.IsTrue((annots[1] as PdfFreeTextAnnotation).Text.Contains("2"));
                Assert.IsTrue((annots[2] as PdfFreeTextAnnotation).Text.Contains("三"));
                Assert.IsTrue((annots[3] as PdfFreeTextAnnotation).Text.Contains("4"));
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", PdfAnnotationSubtype.FreeText)]
        public void SetText_WhenCalled_ShouldShowTextAnnotation(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfFreeTextAnnotation();
                var bounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                annot.AnnotBox = new RectangleF(bounds.Width / 2 - annotSize.Width / 2, bounds.Height - 50 - annotSize.Height / 2, annotSize.Width, annotSize.Height);
                annot.AnnotColor = Color.Red;
                annot.Text = "This is a free tect first";
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                Pdfium.Instance.Save(pageReader.Document, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(1, annots.Count);
            });
        }
        #endregion

        #region Highlight

        #endregion

        #region Ink

        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        public void GetInks_WhenCalled_ShouldGetInk(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(5, annots.Count);
                var firstAnnot = annots[0] as PdfInkAnnotation;
                firstAnnot.GetInks();
                Assert.AreEqual(1, firstAnnot.Inks.Count);
            });
        }

        #endregion

        #region LineAnnotation

        [TestCase("Docs/mytest_4_lineannotation.pdf", PdfAnnotationSubtype.Line)]
        public void StartLocation_WhenCalledWithLineAnnotionPdf_ShouldGetLinePosition(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var firstAnnot = annots[0] as PdfLineAnnotation;
                Assert.IsTrue(firstAnnot.StartLocation != default);
            });
        }

        #endregion
    }
}
