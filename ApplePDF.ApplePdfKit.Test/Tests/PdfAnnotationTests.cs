using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using Xunit;
using PointF = System.Drawing.PointF;
using Color = System.Drawing.Color;
#if IOS || MACCATALYST
using Lib = ApplePDF.PdfKit.PdfKitLib;
#else
using Lib = ApplePDF.PdfKit.PdfiumLib;
#endif

namespace ApplePDF.ApplePdfKit.Test.Tests
{
    public class PdfAnnotationTests
    {
        private readonly ILib _fixture = Lib.Instance;
        string[] AllAssets = typeof(PdfAnnotationTests).Assembly.GetManifestResourceNames();
        PdfDocument LoadPdfDocument(string path, string password)
        {
            //原资源名 Docs/Docnet/simple_0.pdf
            //嵌入后的资源名 ApplePDF.ApplePdfKit.Test.Docnet.simple_0.pdf
            if (path.Contains("Docs/"))
            {
                path = path.Replace("Docs/", "");
            }
            path = path.Replace('/', '.');
            var fileName = AllAssets.First((s) => s.Contains(path));
            return _fixture.LoadPdfDocument(ReadEmbedAssetBytes(fileName), password);
        }

        /// <summary>
        /// 从EmbeddedResource加载字体数据
        /// </summary>
        /// <param name="path">如Fonts/name.ttf</param>
        /// <returns></returns>
        byte[] LoadFont(string path)
        {
            path = path.Replace('/', '.');
            var fileName = AllAssets.First((s) => s.Contains(path));
            return ReadEmbedAssetBytes(fileName);
        }

        public byte[] ReadEmbedAssetBytes(string resourcePath = "foler.fileName.extention")
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(resourcePath))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        public PdfAnnotationTests()
        {

        }

        [Theory]
        [InlineData("Docs/mytest/mytest_4_highlightannotation.pdf", PdfAnnotationSubtype.Highlight)]
        [InlineData("Docs/mytest/mytest_4_unknow_underlineannotation.pdf", PdfAnnotationSubtype.Unknow)]
        [InlineData("Docs/mytest/mytest_4_unknow_strikeoutannotation.pdf", PdfAnnotationSubtype.Unknow)]
        [InlineData("Docs/mytest/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [InlineData("Docs/mytest/mytest_4_freetextannotation.pdf", PdfAnnotationSubtype.FreeText)]
        [InlineData("Docs/mytest/mytest_4_rectangleannotation.pdf", PdfAnnotationSubtype.Square)]
        [InlineData("Docs/mytest/mytest_4_linkannotation.pdf", PdfAnnotationSubtype.Link)]
        public void AnnotationTypeTest(string filePath, PdfAnnotationSubtype type)
        {
            var doc = LoadPdfDocument(filePath, null);
            var pageReader = doc.GetPage(0);
            var annot = pageReader.GetAnnotations()[0];
            Assert.Equal(type, annot.AnnotationType);
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_4_unknow_underlineannotation.pdf", PdfAnnotationSubtype.Unknow)]
        [InlineData("Docs/mytest/mytest_4_unknow_strikeoutannotation.pdf", PdfAnnotationSubtype.Unknow)]
        public void PdfUnknowAnnotation_CustomAnnotationTypeTest(string filePath, PdfAnnotationSubtype type)
        {
            var doc = LoadPdfDocument(filePath, null);
            var pageReader = doc.GetPage(0);
            var annot = pageReader.GetAnnotations()[0];
            Assert.Equal(type, annot.AnnotationType);
            var customType = (annot as PdfUnknowAnnotation).CustomAnnotationType;
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_chinese.pdf", "这是一个中文注释")]
        public void PdfHightlightAnnotation_PopupAnnotation_Text_Chinese_Test(string filePath, string text)
        {
            var doc = LoadPdfDocument(filePath, null);
            var pageReader = doc.GetPage(0);
            var annots = pageReader.GetAnnotations();
            Assert.True(annots != null && annots.Length == 1, "应该有且只有一个注释");
            var annot = annots[0];
            Assert.True(annot.AnnotationType == PdfAnnotationSubtype.Highlight, "应该是Highlight注释");
            var isContain = (annot as PdfKit.Annotation.PdfHighlightAnnotation).PopupAnnotation.Text?.Contains(text);
            Assert.True(isContain, $"应该包含文本\"{text}\"");
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.FreeText, "是FreeText")]
        public void AddAnnotation_FreeText_Test(string filePath, PdfAnnotationSubtype subtype, string text)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var newAnnot = page.AddAnnotation(subtype) as PdfFreeTextAnnotation;
            newAnnot.AnnotBox = PdfRectangleF.FromLTRB(50, 200, 250, 100);
            newAnnot.Text = text;
            newAnnot.TextColor = color;
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_FreeText_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
            Assert.Equal(text, (resultAnnot as IPdfFreeTextAnnotation).Text);
            var resultColor = (resultAnnot as IPdfFreeTextAnnotation).TextColor.Value;
            Assert.True(color.R == resultColor.R && color.G == resultColor.G && color.B == resultColor.B, $"Excepted {color}, Actual {resultColor}");
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Highlight)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Underline)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Squiggly)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.StrikeOut)]
        public void AddAnnotation_Markup_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rects = page.GetCharactersBounds(1, 10);
            var rect = PdfRectangleF.FromLTRB(rects[0].Left, rects[0].Top, rects[rects.Length - 1].Right, rects[rects.Length - 1].Bottom);
            var newAnnot = page.AddAnnotation(subtype) as PdfMarkupAnnotation;
            newAnnot.AnnotBox = rect;
            newAnnot.Location = rect;
            newAnnot.StrokeColor = color;
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Markup_Test)}_{subtype.ToString()}_Result.pdf");
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
            if (subtype == resultAnnot.AnnotationType)
            {
                Assert.NotNull((resultAnnot as PdfMarkupAnnotation)?.Location);
            }
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Circle)]
        public void AddAnnotation_Circle_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rect = PdfRectangleF.FromLTRB(50, 300, 300, 50);
            var newAnnot = page.AddAnnotation(subtype) as PdfCircleAnnotation;
            newAnnot.AnnotBox = rect;
            newAnnot.StrokeColor = color;
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Circle_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink)]
        public void AddAnnotation_Ink_Path_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rect = PdfRectangleF.FromLTRB(50, 300, 300, 50);
            var newAnnot = page.AddAnnotation(subtype) as PdfInkAnnotation;
            newAnnot.AnnotBox = rect;
#if ANDROID || WINDOWS
            var path = PdfPagePathObj.Create(new PointF(rect.Left, rect.Top));
            path.AddPath(new List<PdfSegmentPath>()
            {
                //new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.MoveTo, Position = new PointF(rect.Left, rect.Top)},
                new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(rect.Right, rect.Top)},
                new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(rect.Right, rect.Bottom)},
                //new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(rect.Left, rect.Bottom), IsCloseToStart = true }
            });
            newAnnot.AppendObj(path);
            path.SetStrokeColor(color);
            path.SetFillColor(color);
            path.SetStrokeWidth(5);
            path.SetDrawMode();
#endif
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Ink_Path_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
#if ANDROID || WINDOWS
            var objs = (resultAnnot as PdfInkAnnotation).PdfPageObjs;
            Assert.NotNull(objs);
#endif
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, true)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, false)]
        public void AddAnnotation_Ink_Points_Test(string filePath, PdfAnnotationSubtype subtype, bool defaultWay)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rectBorder = PdfRectangleF.FromLTRB(50-5, 300+5, 300+5, 50-5);
            var rect = PdfRectangleF.FromLTRB(50, 300, 300, 50);
            var newAnnot = page.AddAnnotation(subtype) as PdfInkAnnotation;
            newAnnot.AnnotBox = rectBorder;
#if ANDROID || WINDOWS
            var points = new List<PointF>()
            { 
                rect.LTPoint,
                rect.RTPoint,
                rect.RBPoint,
                rect.LBPoint,
                rect.LTPoint
            };
            newAnnot.AddInkPoints(points, defaultWay);
            newAnnot.InkColor = color;
#endif
#if ANDROID || WINDOWS
            page.SaveNewContent();
            //Assert.NotNull(newAnnot.GetInkPoints());
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Ink_Points_Test)}{(defaultWay?"":"_ByKey")}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
#if ANDROID || WINDOWS
            var resultPoints = (resultAnnot as PdfInkAnnotation).GetInkPoints();
            Assert.NotNull(resultPoints);
#endif
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Highlight, PdfAnnotationSubtype.Popup)]
        public void AddAnnotation_Highlight_Popup_Test(string filePath, PdfAnnotationSubtype parentSubtype, PdfAnnotationSubtype subtype)
        {
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rects = page.GetCharactersBounds(1, 10);
            var rect = PdfRectangleF.FromLTRB(rects[0].Left, rects[0].Top, rects[rects.Length - 1].Right, rects[rects.Length - 1].Bottom);
            var newAnnot = page.AddAnnotation(parentSubtype) as PdfHighlightAnnotation;
            newAnnot.AnnotBox = rect;
            newAnnot.Location = rect;
            var parentAnnot = newAnnot;
            var popupAnnot = parentAnnot.AddPopupAnnotation();
            popupAnnot.Text = "有Popup";
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Highlight_Popup_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(parentSubtype, resultAnnot.AnnotationType);
            Assert.Equal(subtype, (resultAnnot as PdfHighlightAnnotation).PopupAnnotation.AnnotationType);
        }
    }
}
