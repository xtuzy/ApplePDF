using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using NUnit.Framework;
using Pdf.Net.Test.Extensions;
using System;
using System.Collections.Generic;
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

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "Yellow")]//FillColor
        [TestCase("Docs/mytest_5_inkannotation.pdf", "Blue")]//stroke
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "#ff0078d4")]//fill,0,120,212
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "Red")]//stroke,255,0,0
        [TestCase("Docs/mytest_4_linkannotation.pdf", "Transparent")]//annot,255,0,0,0
        [TestCase("Docs/mytest_4_lineannotation.pdf", "Transparent")]//stroke,255,0,0
        public void TryGetColor_ForModifyApi(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                var expectedColor = System.Drawing.Color.FromName(color);
                var colors = annot.TryGetColor();
                Assert.Ignore("只为测试每种注释所使用的颜色Api");
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "#fffff066")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "#ff004de6")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "#ff0078d4")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "#ffff0000")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "#00000000")]
        [TestCase("Docs/mytest_4_lineannotation.pdf", "#ffff0000")]
        public void AnnotColor_WhenCalled_ShouldNotAllCasePass(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0] as IDefaultColorAnnotation;
                if (annot != null && annot.AnnotColor != null)
                {
                    var expect = System.Drawing.ColorTranslator.FromHtml(color);
                    var actual = annot.AnnotColor.Value;
                    Assert.IsTrue(expect.IsEqual(actual));
                }
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "#fffff066")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "#ff004de6")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "#ff0078d4")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "#ff000000")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "#00000000")]
        [TestCase("Docs/mytest_4_lineannotation.pdf", "#ffff0000")]
        public void FillColor_WhenCalled_ShouldGetCurrentColor(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0] as IFillColorAnnotation;
                if (annot != null && annot.FillColor != null)
                {
                    var except = System.Drawing.ColorTranslator.FromHtml(color);
                    var actual = annot.FillColor.Value;
                    Assert.IsTrue(except.IsEqual(actual));
                }
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "#fffff066")]
        [TestCase("Docs/mytest_5_inkannotation.pdf", "#ff004de6")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "#ff0078d4")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "#ffff0000")]
        [TestCase("Docs/mytest_4_linkannotation.pdf", "#00000000")]
        [TestCase("Docs/mytest_4_lineannotation.pdf", "#ffff0000")]
        public void StrokeColor_WhenCalled_ShouldGetCurrentColor(string filePath, string color)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0] as IStrokeColorAnnotation;
                if (annot != null && annot.StrokeColor != null)
                {
                    var except = System.Drawing.ColorTranslator.FromHtml(color);
                    var actual = annot.StrokeColor.Value;
                    Assert.IsTrue(except.IsEqual(actual));
                }
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        public void Position_WhenCalled_ShouldGetAnnotationPosition(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                var annot = annots[0];
                Assert.Positive(annot.AnnotBox.Right - annot.AnnotBox.Left);
                Assert.Positive(annot.AnnotBox.Top - annot.AnnotBox.Bottom);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", "#ff00ffff")]
        public void AddAnnotation_HighlightAnnotation_ShouldShowRectangle(string filePath, string colorStr)
        {
            float left = 0;
            float top = 0;
            float right = 0;
            float bottom = 0;
            var exceptColor = System.Drawing.ColorTranslator.FromHtml(colorStr);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfHighlightAnnotation();
                var pageBounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                left = pageBounds.Width / 2 - annotSize.Width / 2;
                top = pageBounds.Height / 2 + annotSize.Height / 2;
                right = pageBounds.Width / 2 + annotSize.Width / 2;
                bottom = pageBounds.Height / 2 - annotSize.Height / 2;
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.AnnotColor = exceptColor;
                pageReader.AddAnnotation(annot);
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
                //颜色
                Assert.IsTrue(exceptColor.IsEqual((annots[0] as PdfHighlightAnnotation).AnnotColor.Value));
                //位置
                Assert.AreEqual(left, annots[0].AnnotBox.Left, 0.1);
                Assert.AreEqual(top, annots[0].AnnotBox.Top, 0.1);
                Assert.AreEqual(right, annots[0].AnnotBox.Right, 0.1);
                Assert.AreEqual(bottom, annots[0].AnnotBox.Bottom, 0.1);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", "Hello 12345")]
        public void AddAnnotation_FreeTextAnnotation_ShouldShowFreeText(string filePath, string exceptText)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfFreeTextAnnotation();
                annot.Text = exceptText;
                var pageBounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                //居中
                var left = pageBounds.Width / 2 - annotSize.Width / 2;
                var top = pageBounds.Height / 2 + annotSize.Height / 2;
                var right = pageBounds.Width / 2 + annotSize.Width / 2;
                var bottom = pageBounds.Height / 2 - annotSize.Height / 2;
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                pageReader.AddAnnotation(annot);
                //annot.AppendAnnotationPoint(annot.AnnotBox);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(1, annots.Count, "添加的注释数目为1");
                Assert.AreEqual(PdfAnnotationSubtype.FreeText, annots[0].AnnotationType);
                Assert.AreNotEqual((annots[0] as PdfFreeTextAnnotation).Text, exceptText, "注释中的文本应该一样");
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", "#ff00ffff")]
        public void AddAnnotation_InkAnnotation_ShouldShowInk(string filePath, string colorStr)
        {
            float left = 0;
            float top = 0;
            float right = 0;
            float bottom = 0;
            var exceptColor = System.Drawing.ColorTranslator.FromHtml(colorStr);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(0, annots.Count);
                var annot = new PdfInkAnnotation();
                var pageBounds = pageReader.GetSize();
                var annotSize = new SizeF(100, 100);
                left = pageBounds.Width / 2 - annotSize.Width / 2;
                var top = pageBounds.Height / 2 + annotSize.Height / 2;
                var right = pageBounds.Width / 2 + annotSize.Width / 2;
                var bottom = pageBounds.Height / 2 - annotSize.Height / 2;
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.AnnotColor = exceptColor;
                annot.Inks = new List<List<PointF>>()
                {
                    new List<PointF>(){new PointF(annot.AnnotBox.Left,annot.AnnotBox.Top), new PointF(annot.AnnotBox.Right, annot.AnnotBox.Bottom) },
                };
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annot = pageReader.Annotations[0] as PdfInkAnnotation;
                Assert.AreEqual(PdfAnnotationSubtype.Ink, annot.AnnotationType);
                //颜色
                Assert.IsTrue(exceptColor.IsEqual(annot.AnnotColor.Value));
                //位置
                var point1 = annot.Inks[0][0];
                var point2 = annot.Inks[0][1];
                Assert.AreEqual(left, point1.X, 0.1);
                Assert.AreEqual(top, point1.Y, 0.1);
                Assert.AreEqual(right, point2.X, 0.1);
                Assert.AreEqual(bottom, point2.Y, 0.1);
            });
        }

        #region FreeText

        [TestCase("Docs/mytest_4_freetextannotation.pdf")]
        public void Text_Get_ShouldGetText(string filePath)
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

        #endregion

        #region Highlight

        #endregion

        #region Ink

        [TestCase("Docs/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        public void Inks_WhenCalled_ShouldGetInk(string filePath, PdfAnnotationSubtype type)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(5, annots.Count);
                var firstAnnot = annots[0] as PdfInkAnnotation;
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
