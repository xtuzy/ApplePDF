using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;

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

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        [TestCase("Result.pdf", PdfAnnotationSubtype.Highlight)]
        public void AnnotColor_WhenCalled_ShouldNotAllCasePass(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.IsTrue(
                    annot.AnnotColor.Value.A>0 ||annot.AnnotColor.Value.R>0 || annot.AnnotColor.Value.G>0 || annot.AnnotColor.Value.B>0
                    );
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        [TestCase("Result.pdf", PdfAnnotationSubtype.Highlight)]
        public void FillColor_WhenCalled_ShouldNotAllCasePass(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.IsTrue(
                    annot.FillColor.Value.A > 0 || annot.FillColor.Value.R > 0 || annot.FillColor.Value.G > 0 || annot.FillColor.Value.B > 0
                    );
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        [TestCase("Result.pdf", PdfAnnotationSubtype.Highlight)]
        public void StrokeColor_WhenCalled_ShouldNotAllCasePass(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.IsTrue(
                    annot.StrokeColor.Value.A > 0 || annot.StrokeColor.Value.R > 0 || annot.StrokeColor.Value.G > 0 || annot.StrokeColor.Value.B > 0
                    );
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

        [TestCase("Docs/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Link)]
        public void AddAnnotation_WhenCalledWithHighlightAnnotation_ShouldShowRectangleAnnotation(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfHighlightAnnotation(PdfAnnotationSubtype.Highlight);
                var bounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                annot.AnnotBox = new RectangleF(bounds.Width / 2 - annotSize.Width / 2, bounds.Height - 50 - annotSize.Height / 2, annotSize.Width, annotSize.Height);
                //annot.Color = Color.Cyan;
                pageReader.AddAnnotation(annot);
                annot.AppendAnnotationPoint(annot.AnnotBox);
                annot.Dispose();
                Pdfium.Instance.Save(pageReader.Document, "Result.pdf");
                Debug.WriteLine(pageReader.AnnotationCount);
                Assert.Less(0, pageReader.AnnotationCount);
            });
        }

        #region FreeText

        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        public void GetText_WhenCalled_ShouldGetText(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                foreach (var annot in annots)
                {
                    Assert.IsTrue(annot is PdfFreeTextAnnotation);
                    Assert.IsNotEmpty((annot as PdfFreeTextAnnotation).Text);
                    Debug.WriteLine((annot as PdfFreeTextAnnotation).Text);
                }
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Link)]
        public void SetText_WhenCalled_ShouldShowTextAnnotation(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfFreeTextAnnotation(PdfAnnotationSubtype.FreeText);
                var bounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                annot.AnnotBox = new RectangleF(bounds.Width / 2 - annotSize.Width / 2, bounds.Height - 50 - annotSize.Height / 2, annotSize.Width, annotSize.Height);
                annot.AnnotColor = Color.Red;
                annot.Text = "This is a free tect annot";
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Pdfium.Instance.Save(pageReader.Document, "Result.pdf");
                Debug.WriteLine(pageReader.AnnotationCount);
                Assert.Less(0, pageReader.AnnotationCount);
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
                var annot = annots[0];
                if(annot is PdfInkAnnotation)
                {
                    var annotation = (PdfInkAnnotation)annot;
                    annotation.GetInks();
                    Assert.IsNotEmpty(annotation.Inks);
                    Assert.AreEqual(1, annotation.Inks.Count);
                }else
                    Assert.Fail();
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
                var annot = annots[0];
                if (annot is PdfLineAnnotation)
                {
                    var annotation = (PdfLineAnnotation)annot;
                  
                    Assert.IsTrue(annotation.StartLocation!=default);
                }
                else
                    Assert.Fail();
            });
        }


        #endregion
    }
}
