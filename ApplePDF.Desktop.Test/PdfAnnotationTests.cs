using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using NUnit.Framework;
using Pdf.Net.Test.Extensions;
using PDFiumCore;
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
                var annot = pageReader.GetAnnotation(0);
                Assert.AreEqual(type, annot.AnnotationType);
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", "#fffff066", PdfPageObjectTypeFlag.UNKNOWN)]//FillColor
        [TestCase("Docs/mytest_5_inkannotation.pdf", "#ff004de6", PdfPageObjectTypeFlag.PATH)]//stroke
        [TestCase("Docs/mytest_4_freetextannotation.pdf", "#ff0078d4", PdfPageObjectTypeFlag.TEXT)]//fill,0,120,212
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", "#ffff0000", PdfPageObjectTypeFlag.PATH)]//stroke,255,0,0
        [TestCase("Docs/mytest_4_linkannotation.pdf", "#00000000", PdfPageObjectTypeFlag.PATH)]//annot,255,0,0,0
        [TestCase("Docs/mytest_4_lineannotation.pdf", "#ffff0000", PdfPageObjectTypeFlag.PATH)]//stroke,255,0,0
        public void TryGetColor_ForModifyApi(string filePath, string color, PdfPageObjectTypeFlag objType)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0);
                var expectedColor = System.Drawing.Color.FromName(color);
                var colors = annot.TryGetColor(objType);
                Assert.Ignore($"Expect {expectedColor}, AnnotColor {colors.AnnotColor}, FillColor {colors.FillColor}, StrokeColor {colors.StrokeColor}");
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
                var annot = pageReader.GetAnnotation(0) as IColorAnnotation;
                if (annot != null && annot.AnnotColor != null)
                {
                    var expect = System.Drawing.ColorTranslator.FromHtml(color);
                    var actual = annot.AnnotColor.Value;
                    Assert.IsTrue(expect.IsEqual(actual), $"Expect {expect}, Actual {actual}");
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
                var annot = pageReader.GetAnnotation(0) as IPdfPageObjAnnotation;
                if (annot != null && annot.PdfPageObjs != null)
                {
                    foreach (var obj in annot.PdfPageObjs)
                    {
                        var expect = System.Drawing.ColorTranslator.FromHtml(color);
                        if (obj.GetFillColor() != null)
                        {
                            var actual = obj.GetFillColor().Value;
                            Assert.IsTrue(expect.IsEqual(actual), $"Expect {expect}, Actual {actual}");
                        }
                        else
                            Assert.Ignore();
                    }
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
                var annot = pageReader.GetAnnotation(0) as IPdfPageObjAnnotation;
                if (annot != null && annot.PdfPageObjs != null)
                {
                    foreach (var obj in annot.PdfPageObjs)
                    {
                        var expect = System.Drawing.ColorTranslator.FromHtml(color);
                        if (obj.GetStrokeColor() != null)
                        {
                            var actual = obj.GetStrokeColor().Value;
                            Assert.IsTrue(expect.IsEqual(actual), $"Expect {expect}, Actual {actual}");
                        }
                        else
                            Assert.Ignore();
                    }
                }
                else
                    Assert.Ignore();
            });
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf")]
        [TestCase("Docs/mytest_5_inkannotation.pdf")]
        [TestCase("Docs/mytest_4_freetextannotation.pdf")]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf")]
        [TestCase("Docs/mytest_4_linkannotation.pdf")]
        public void AnnotBox_WhenCalled_ShouldGetAnnotationPosition(string filePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0);
                Assert.Positive(annot.AnnotBox.Right - annot.AnnotBox.Left);
                Assert.Positive(annot.AnnotBox.Top - annot.AnnotBox.Bottom);
            });
        }

        [TestCase("Docs/mytest_5_inkannotation.pdf")]
        public void APString_WhenCalled_ShouldGetAnnotation(string filePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annotFirst = pageReader.GetAnnotation(0);
                var annot = annotFirst as PdfInkAnnotation;
                var objs = annot.PdfPageObjs as List<PdfPageObj>;
                var obj = objs[0] as PdfPagePathObj;
                var paths = obj.GetPath();
                foreach (var path in paths)
                {
                    switch (path.Type)
                    {
                        case PdfSegmentFlag.FPDF_SEGMENT_UNKNOWN:
                            break;
                        case PdfSegmentFlag.FPDF_SEGMENT_LINETO:
                            Console.WriteLine($"LINETO {path.Position}");
                            break;
                        case PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO:
                            var tempPath = path as PdfBezierSegmentPath;
                            Console.WriteLine($"BEZIER {tempPath.ControlPoint1} {tempPath.ControlPoint2} {tempPath.Position}");
                            break;
                        case PdfSegmentFlag.FPDF_SEGMENT_MOVETO:
                            Console.WriteLine($"MOVETO {path.Position}");
                            break;
                    }
                }
                var ap = annot.GetAppearenceStr();
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 100, 300, 200, 100, "#ff00ffff")]
        [TestCase("Docs/Pdfium/hello_world.pdf", 100, 300, 200, 100, "#ff00ffff")]
        public void AddAnnotation_HighlightAnnotation_ShouldShowRectangle(string filePath, float left, float top, float right, float bottom, string expectColorStr)
        {
            var expectColor = System.Drawing.ColorTranslator.FromHtml(expectColorStr);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfHighlightAnnotation();
                var pageBounds = pageReader.GetSize();
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.AnnotColor = expectColor;
                annot.HighlightLocation.Add(annot.AnnotBox);
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var annot = pageReader.GetAnnotation(0);
                Assert.AreEqual(PdfAnnotationSubtype.Highlight, annot.AnnotationType);
                //颜色
                Assert.IsTrue(expectColor.IsEqual((annot as PdfHighlightAnnotation).AnnotColor.Value));
                //位置
                Assert.AreEqual(left, annot.AnnotBox.Left, 0.1);
                Assert.AreEqual(top, annot.AnnotBox.Top, 0.1);
                Assert.AreEqual(right, annot.AnnotBox.Right, 0.1);
                Assert.AreEqual(bottom, annot.AnnotBox.Bottom, 0.1);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 35, 165, 53, 150, "Hello 12345")]
        [TestCase("Docs/Pdfium/hello_world.pdf", 35, 165, 53, 150, "Hello 12345")]
        public void AddAnnotation_FreeTextAnnotation_ShouldShowFreeText(string filePath, float left, float top, float right, float bottom, string exceptText)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfFreeTextAnnotation();
                annot.Text = exceptText;
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0);
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                Assert.AreEqual(PdfAnnotationSubtype.FreeText, annot.AnnotationType);
                Assert.AreEqual(exceptText, (annot as PdfFreeTextAnnotation).Text, "注释中的文本应该一样");
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 35, 165, 53, 150, "Hello! This is a customized content.好", "Red")]
        [TestCase("Docs/Pdfium/hello_world.pdf", 35, 165, 53, 150, "Hello! This is a customized content.", "Red")]
        public void AddAnnotation_TextAnnotation_ShouldShowText(string filePath, float left, float top, float right, float bottom, string exceptText, string colorName)
        {
            var expectColor = Color.FromName(colorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfTextAnnotation();
                annot.Text = exceptText;
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.AnnotColor = expectColor;
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0);
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                Assert.AreEqual(PdfAnnotationSubtype.Text, annot.AnnotationType);
                Assert.IsTrue(expectColor.IsEqual((annot as PdfTextAnnotation).AnnotColor.Value));
                Assert.AreEqual(exceptText, (annot as PdfTextAnnotation).Text, "注释中的文本应该一样");
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 100, 300, 200, 100, "Red")]
        [TestCase("Docs/Pdfium/hello_world.pdf", 100, 300, 200, 100, "Red")]
        public void AddAnnotation_UnderlineAnnotation_ShouldShowUnderline(string filePath, float left, float top, float right, float bottom, string colorName)
        {
            var expectColor = Color.FromName(colorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfUnderlineAnnotation();
                var rects = pageReader.GetCharactersBounds(0, 10);
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.UnderlineLocation.AddRange(rects);
                annot.AnnotColor = expectColor;
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                Assert.AreEqual(PdfAnnotationSubtype.Underline, pageReader.GetAnnotation(0).AnnotationType, "添加的注释数目为1");
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0);
                Assert.AreEqual(1, pageReader.AnnotationCount, "添加的注释数目为1");
                Assert.AreEqual(PdfAnnotationSubtype.Underline, annot.AnnotationType);
                Assert.IsTrue(expectColor.IsEqual((annot as PdfUnderlineAnnotation).AnnotColor.Value));
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 100, 300, 200, 100, "Red")]
        [TestCase("Docs/Pdfium/hello_world.pdf", 100, 300, 200, 100, "Red")]
        public void AddAnnotation_LineAnnotation_ShouldThrowNotImplementedException(string filePath, float left, float top, float right, float bottom, string colorName)
        {
            var expectColor = Color.FromName(colorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfLineAnnotation();
                var rects = pageReader.GetCharactersBounds(0, 10);
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                Assert.Catch<NotImplementedException>(() =>
                {
                    pageReader.AddAnnotation(annot);
                });
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 100, 300, 200, 100, "#ff00ffff")]
        public void AddAnnotation_InkAnnotation_ByInkListPaths_ShouldShowInk(string filePath, float left, float top, float right, float bottom, string colorStr)
        {
            var exceptColor = System.Drawing.ColorTranslator.FromHtml(colorStr);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfInkAnnotation();
                var pageBounds = pageReader.GetSize();
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                annot.AnnotColor = exceptColor;
                annot.InkListPaths = new List<List<PointF>>()
                {
                    new List<PointF>(){new PointF(annot.AnnotBox.Left, annot.AnnotBox.Top), new PointF(annot.AnnotBox.Right, annot.AnnotBox.Bottom),new PointF(100, 100) },
                 };
                pageReader.AddAnnotation(annot);
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                Assert.AreEqual(PdfAnnotationSubtype.Ink, annot.AnnotationType);
                //颜色
                Assert.IsTrue(exceptColor.IsEqual(annot.AnnotColor.Value));
                //位置
                var point1 = annot.InkListPaths[0][0];
                var point2 = annot.InkListPaths[0][1];
                Assert.AreEqual(left, point1.X, 0.1);
                Assert.AreEqual(top, point1.Y, 0.1);
                Assert.AreEqual(right, point2.X, 0.1);
                Assert.AreEqual(bottom, point2.Y, 0.1);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 100, 300, 200, 100, "#ff00ffff")]
        public void AddAnnotation_InkAnnotation_ByPageObj_ShouldShowInk(string filePath, float left, float top, float right, float bottom, string colorStr)
        {
            var exceptColor = System.Drawing.ColorTranslator.FromHtml(colorStr);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(0, pageReader.AnnotationCount);
                var annot = new PdfInkAnnotation();
                var pageBounds = pageReader.GetSize();
                annot.AnnotBox = PdfRectangleF.FromLTRB(left, top, right, bottom);
                var pathObj = PdfPagePathObj.Create(new PointF(left, top));
                pathObj.AddPath(new List<PdfSegmentPath>()
                {
                    new PdfSegmentPath(){Type=PdfSegmentFlag.FPDF_SEGMENT_MOVETO,Position = new PointF(left,top)},
                    new PdfSegmentPath(){Type=PdfSegmentFlag.FPDF_SEGMENT_LINETO,Position = new PointF(right,bottom)},
                });
                annot.PdfPageObjs = new List<PdfPagePathObj>() { pathObj };
                pathObj.SetStrokeColor(exceptColor);
                pathObj.SetStrokeWidth(5);
                pathObj.SetDrawMode();
                pageReader.AddAnnotation(annot);
                annot.PdfPageObjs = null;
                annot.Dispose();
                Assert.AreEqual(1, pageReader.AnnotationCount);
                var newAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                var newObjs = (newAnnot.PdfPageObjs as List<PdfPageObj>)[0];
                var newPaths = (newObjs as PdfPagePathObj)?.GetPath();
                Assert.AreEqual(2, newPaths.Count);
                Assert.AreEqual(PdfSegmentFlag.FPDF_SEGMENT_MOVETO, newPaths[0].Type);
                Assert.AreEqual(PdfSegmentFlag.FPDF_SEGMENT_LINETO, newPaths[1].Type);
                newAnnot.PdfPageObjs = null;
                newAnnot.Dispose();
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf", PdfSaveFlag.DefaultInTest);
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var text = pageReader.Text;
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                var pointPaths = firstAnnot.InkListPaths;
                //颜色
                var objs = firstAnnot.PdfPageObjs as List<PdfPageObj>;
                Assert.IsNotNull(objs);
                Assert.AreEqual(1, objs.Count);

                var obj = objs[0];
                var pathObj = obj as PdfPagePathObj;
                if (pathObj != null)
                {
                    var actualColor = pathObj.GetStrokeColor();
                    Assert.IsTrue(exceptColor.IsEqual(actualColor.Value));
                    var path = pathObj.GetPath();
                    var point1 = path[0].Position;
                    var point2 = path[1].Position;
                    Assert.AreEqual(left, point1.X, 0.1);
                    Assert.AreEqual(top, point1.Y, 0.1);
                    Assert.AreEqual(right, point2.X, 0.1);
                    Assert.AreEqual(bottom, point2.Y, 0.1);
                }
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf")]
        public void AddAnnotation_InkAnnotation_ByAPString_WhenCalled_ShouldGetAnnotation(string filePath)
        {
            var apStream = "0.000 0.302 0.902 RG 1.50 w  1 J 1 j 97.256050 681.571350 m 97.159363 681.329773 96.901207 680.974609 96.675926 680.121704 c 96.450645 679.268799 96.155640 677.950745 95.904358 676.453979 c 95.653084 674.957214 95.389801 673.478455 95.168243 671.140991 c 94.946678 668.803589 94.744019 665.611267 94.574982 662.429321 c 94.405945 659.247437 94.268051 655.402771 94.154015 652.049683 c 94.039970 648.696533 93.957901 645.469238 93.890755 642.310547 c 93.823593 639.151917 93.783394 635.912109 93.751099 633.097656 c 93.718803 630.283325 93.706360 627.684692 93.696983 625.424194 c 93.687607 623.163757 93.691170 621.327515 93.694847 619.534973 c 93.698524 617.742432 93.661140 616.309570 93.719055 614.669189 c 93.776985 613.028809 93.918846 611.219727 94.042389 609.692688 c 94.165932 608.165588 94.390663 606.204407 94.460320 605.506714 c S";

            using (var doc = _fixture.LoadPdfDocument(filePath, null))
            {
                using (var pageReader = doc.GetPage(0))
                {
                    var annot = new PdfInkAnnotation();
                    annot.AnnotBox = PdfRectangleF.FromLTRB(90, 684, 95, 600);
                    pageReader.AddAnnotation(annot);
                    var success = annot.SetAppearenceStr(apStream);
                    //pageReader.SaveNewContent();
                    var ap = pageReader.GetAnnotation(0).GetAppearenceStr();
                    Assert.AreEqual(apStream, ap);
                }
                var savesuccess = Pdfium.Instance.Save(doc, "Result.pdf", PdfSaveFlag.DefaultInTest);
            }

            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annotFirst = pageReader.GetAnnotation(0);
                var annot = annotFirst as PdfInkAnnotation;
                var ap = annot.GetAppearenceStr();
                Assert.AreEqual(apStream, ap);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf")]
        public void AddAnnotation_StampAnnotation_ByAPString_WhenCalled_ShouldGetAnnotation(string filePath)
        {
            var apStream = "0.000 0.302 0.902 RG 1.50 w  1 J 1 j 97.256050 681.571350 m 97.159363 681.329773 96.901207 680.974609 96.675926 680.121704 c 96.450645 679.268799 96.155640 677.950745 95.904358 676.453979 c 95.653084 674.957214 95.389801 673.478455 95.168243 671.140991 c 94.946678 668.803589 94.744019 665.611267 94.574982 662.429321 c 94.405945 659.247437 94.268051 655.402771 94.154015 652.049683 c 94.039970 648.696533 93.957901 645.469238 93.890755 642.310547 c 93.823593 639.151917 93.783394 635.912109 93.751099 633.097656 c 93.718803 630.283325 93.706360 627.684692 93.696983 625.424194 c 93.687607 623.163757 93.691170 621.327515 93.694847 619.534973 c 93.698524 617.742432 93.661140 616.309570 93.719055 614.669189 c 93.776985 613.028809 93.918846 611.219727 94.042389 609.692688 c 94.165932 608.165588 94.390663 606.204407 94.460320 605.506714 c S";

            using (var doc = _fixture.LoadPdfDocument(filePath, null))
            {
                using (var pageReader = doc.GetPage(0))
                {
                    var annot = new PdfStampAnnotation();
                    annot.AnnotBox = PdfRectangleF.FromLTRB(90, 684, 95, 600);
                    pageReader.AddAnnotation(annot);
                    var success = annot.SetAppearenceStr(apStream);
                    var ap = pageReader.GetAnnotation(0).GetAppearenceStr();
                    Assert.AreEqual(apStream, ap);
                }
                Pdfium.Instance.Save(doc, "Result.pdf", PdfSaveFlag.DefaultInTest);
            }

            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var annotFirst = pageReader.GetAnnotation(0);
                var annot = annotFirst as PdfStampAnnotation;
                var ap = annot.GetAppearenceStr();
                Assert.AreEqual(apStream, ap);
            });
        }

        #region FreeText

        [TestCase("Docs/mytest_4_freetextannotation.pdf")]
        public void FreeTextAnnot_GetText_ShouldGetText(string filePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(4, pageReader.AnnotationCount);
                Assert.IsTrue((pageReader.GetAnnotation(0) as PdfFreeTextAnnotation).Text.Contains("1"));
                Assert.IsTrue((pageReader.GetAnnotation(1) as PdfFreeTextAnnotation).Text.Contains("2"));
                Assert.IsTrue((pageReader.GetAnnotation(2) as PdfFreeTextAnnotation).Text.Contains("三"));
                Assert.IsTrue((pageReader.GetAnnotation(3) as PdfFreeTextAnnotation).Text.Contains("4"));
            });
        }

        #endregion

        #region Highlight

        #endregion

        #region Ink

        [TestCase("Docs/mytest_5_inkannotation.pdf", 5)]
        public void InkAnnot_InkListPaths_Get_WhenCalled_ShouldGetRightCountOfInk(string filePath, int inkCount)
        {
            try
            {
                ExecuteForDocument(filePath, null, 0, pageReader =>
                {
                    Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                    var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                    var pointPaths = firstAnnot.InkListPaths;
                    Assert.AreEqual(1, pointPaths.Count);
                    foreach (var path in pointPaths)
                    {
                        foreach (var point in path)
                        {
                            Assert.IsTrue(point.X == 0 && point.Y == 0, "Have point is (0.0), maybe this path is not right");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestCase("Docs/Pdfium/ink_annot.pdf", 2)]
        public void InkAnnot_InkListPaths_Get_Pdfium_WhenCalled_ShouldGetRightPointsOfInk(string filePath, int inkCount)
        {
            try
            {
                ExecuteForDocument(filePath, null, 0, pageReader =>
                {
                    Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                    var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                    Assert.AreEqual(1, firstAnnot.InkListPaths.Count);
                    var point0 = firstAnnot.InkListPaths[0][0];
                    var point1 = firstAnnot.InkListPaths[0][1];
                    var point2 = firstAnnot.InkListPaths[0][2];
                    Assert.AreEqual(159.0f, point0.X, 0.1);
                    Assert.AreEqual(296.0f, point0.Y, 0.1);
                    Assert.AreEqual(350.0f, point1.X, 0.1);
                    Assert.AreEqual(411.0f, point1.Y, 0.1);
                    Assert.AreEqual(472.0f, point2.X, 0.1);
                    Assert.AreEqual(243.42f, point2.Y, 0.1);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [TestCase("Docs/mytest_5_inkannotation.pdf", 5, "Red")]
        [TestCase("Docs/Pdfium/ink_annot.pdf", 2, "Red")]
        public void InkAnnot_UpdateAnnotColor_WhenCalled_ShouldGetRightColorOfInk(string filePath, int inkCount, string colorName)
        {
            var exceptColor = Color.FromName(colorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                var colors = firstAnnot.TryGetColor(PDFiumCore.PdfPageObjectTypeFlag.PATH);
                firstAnnot.AnnotColor = exceptColor;
                firstAnnot.UpdateInkListPathsAnnotColor();
                firstAnnot.Dispose();
                colors = pageReader.GetAnnotation(0).TryGetColor(PDFiumCore.PdfPageObjectTypeFlag.PATH);
                Assert.IsTrue(exceptColor.IsEqual(colors.AnnotColor.Value));
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf");
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                //颜色
                Assert.IsTrue(exceptColor.IsEqual(firstAnnot.AnnotColor.Value));
            });
        }

        [TestCase("Docs/mytest_5_inkannotation.pdf", 5, "#ff004de6")]
        [TestCase("Docs/Pdfium/ink_annot.pdf", 2, "#ffe500")]
        public void InkAnnot_PdfPageObjs_Get_WhenCalled_ShouldGetInfoFormPdfPageObjsOfInk(string filePath, int inkCount, string expectColorName)
        {
            var expectColor = ColorTranslator.FromHtml(expectColorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                var objs = firstAnnot.PdfPageObjs;
                if (objs != null)
                {
                    foreach (var obj in objs)
                    {
                        var pathObj = obj as PdfPagePathObj;
                        if (pathObj != null)
                        {
                            var fillColor = pathObj.GetFillColor();
                            var strokeColor = pathObj.GetStrokeColor();
                            Assert.IsTrue(expectColor.IsEqual(fillColor.Value) || expectColor.IsEqual(strokeColor.Value));
                            var paths = pathObj.GetPath();
                            foreach (var path in paths)
                            {
                                if (path.Type == PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO)
                                {
                                    var tempPath = path as PdfBezierSegmentPath;

                                    if (tempPath.ControlPoint2.X == tempPath.Position.X &&
                                       tempPath.ControlPoint2.Y == tempPath.Position.Y)
                                    {
                                        Assert.Fail($"{filePath}:贝塞尔曲线控制点2和结束点一样, 应该是错了");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Assert.Fail($"{filePath}:PdfPageObjs of annot is not PdfPagePathObj.");
                        }
                    }
                }
                else
                {
                    Assert.Fail($"{filePath}:No PdfPageObjs in annot. Maybe the annot use InkList");
                }
                firstAnnot.PdfPageObjs = null;
                firstAnnot.Dispose();
            });
        }

        [TestCase("Docs/mytest_5_inkannotation.pdf", 5, "Red")]
        [TestCase("Docs/Pdfium/ink_annot.pdf", 2, "Red")]
        public void InkAnnot_PdfPageObjs_UpdateColor_WhenCalled_ShouldSetInfoForInk(string filePath, int inkCount, string colorName)
        {
            var exceptColor = Color.FromName(colorName);
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                var objs = firstAnnot.PdfPageObjs;
                if (objs != null)
                {
                    foreach (var obj in objs)
                    {
                        var pathObj = obj as PdfPagePathObj;
                        if (pathObj != null)
                        {
                            pathObj.SetStrokeColor(exceptColor);
                            firstAnnot.UpdateObj(pathObj);
                        }
                        else
                        {
                            Assert.Fail($"{filePath}:PdfPageObjs of annot is not PdfPagePathObj.");
                        }
                    }
                }
                else
                {
                    Assert.Fail($"{filePath}:No PdfPageObjs in annot. Maybe the annot use InkList");
                }
                firstAnnot.Dispose();
                var doc = pageReader.Document;
                Pdfium.Instance.Save(doc, "Result.pdf", PdfSaveFlag.DefaultInTest);
            });
            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                Assert.AreEqual(inkCount, pageReader.AnnotationCount);
                var firstAnnot = pageReader.GetAnnotation(0) as PdfInkAnnotation;
                //颜色
                var objs = firstAnnot.PdfPageObjs;
                if (objs != null)
                {
                    foreach (var obj in objs)
                    {
                        var pathObj = obj as PdfPagePathObj;
                        if (pathObj != null)
                        {
                            var actual = pathObj.GetStrokeColor();
                            Assert.IsTrue(exceptColor.IsEqual(actual.Value));
                        }
                        else
                        {
                            Assert.Fail($"{filePath}:PdfPageObjs of annot is not PdfPagePathObj.");
                        }
                    }
                }
                else
                {
                    Assert.Fail($"Result.pdf:No PdfPageObjs in annot. Maybe the annot use InkList");
                }
            });
        }

        #endregion

        #region LineAnnotation

        [TestCase("Docs/mytest_4_lineannotation.pdf", 110, 715, 179, 721)]
        [TestCase("Docs/Pdfium/line_annot.pdf", 159, 296, 472, 243)]
        public void LineAnnot_Get_StartLocation_ShouldGetLinePosition(string filePath, int x1, int y1, int x2, int y2)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var firstAnnot = pageReader.GetAnnotation(0) as PdfLineAnnotation;
                Assert.AreEqual(x1, (int)firstAnnot.StartLocation.X);
                Assert.AreEqual(y1, (int)firstAnnot.StartLocation.Y);
                Assert.AreEqual(x2, (int)firstAnnot.EndLocation.X);
                Assert.AreEqual(y2, (int)firstAnnot.EndLocation.Y);
            });
        }
        #endregion
    }
}
