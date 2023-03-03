using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using Xunit;
using PointF = System.Drawing.PointF;
using Color = System.Drawing.Color;
#if IOS || MACCATALYST
using Foundation;
using iOSPdfKit = PdfKit;
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

        #region Hightlight

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

        #endregion

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
#if IOS || MACCATALYST
            newAnnot.FillColor = Color.Green;
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Circle_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
        }

        #region Ink

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink)]
        public void AddAnnotation_Ink_Path_CURD_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            //test data
            var strokeColor = Color.Green; 
            var fillColor = Color.Blue;
            var strokeWidth = 5;
            var pathRect = PdfRectangleF.FromLTRB(50, 300, 300, 50);
            var segments = new List<PdfSegmentPath>()
            {
                new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.MoveTo, Position = new PointF(pathRect.Left - pathRect.Left, pathRect.Top - pathRect.Bottom)},
                new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(pathRect.Right - pathRect.Left, pathRect.Top - pathRect.Bottom)},
                new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(pathRect.Right - pathRect.Left, pathRect.Bottom - pathRect.Bottom)},
                //new PdfSegmentPath(){Type = PdfSegmentPath.SegmentFlag.LineTo, Position = new PointF(pathRect.Left - pathRect.Left, pathRect.Bottom - pathRect.Bottom), IsCloseToStart = true }
            };
            var annotBox = PdfRectangleF.FromLTRB(49, 501, 501, 49);
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var size = page.GetSize();
            var newAnnot = page.AddAnnotation(subtype) as PdfInkAnnotation;
            newAnnot.AnnotBox = annotBox;
#if ANDROID || WINDOWS
            newAnnot = page.GetAnnotations()[0] as PdfInkAnnotation;
            var path = PdfPagePathObj.Create(new PointF(pathRect.Left, pathRect.Bottom));
            path.StrokeColor = strokeColor;
            path.FillColor = fillColor;
            path.StrokeWidth = 5;
            path.LineCap = PdfPagePathObj.PdfLineCap.Square;
            path.LineJoin = PdfPagePathObj.PdfLineJoin.Round;
            //相对路径，原点是/Rect
            List<PointF> points = path.GenerateInkPoints(segments, newAnnot.AnnotBox.LBPoint);
            path.AddPath(segments, newAnnot.AnnotBox.LBPoint);
            path.SetDrawMode(true, PdfPagePathObj.PdfFillMode.None);
            newAnnot.AppendObj(path);
            //生成/AP时也需要设置InkPoints
            newAnnot.AddInkPoints(points);
#elif IOS || MACCATALYST
            var path = new UIKit.UIBezierPath();
            path.AddPath(segments);
            path.LineWidth = 5;
            path.LineCapStyle = CoreGraphics.CGLineCap.Square;
            path.LineJoinStyle = CoreGraphics.CGLineJoin.Round;
            newAnnot.AddPath(path);
            newAnnot.StrokeColor = strokeColor;
            newAnnot.FillColor = fillColor;
#endif
            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Ink_Path_CURD_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
#if ANDROID || WINDOWS
            var ink = resultAnnot as PdfInkAnnotation;
            var resultinkpoints = ink.GetInkPoints();
            var objs = ink.PdfPageObjs;
            Assert.NotNull(objs);
            var resultInk = objs[0] as PdfPagePathObj; 
            
            var inkPointFirst = ink.GetInkPoints()[0][0];
            var paths = resultInk.GetPath();
            if (paths[0].Type == PdfSegmentPath.SegmentFlag.MoveTo)
            {
                var pathFirst = paths[0].Position;
                if ((int)pathFirst.X == (int)inkPointFirst.X && (int)pathFirst.Y == (int)inkPointFirst.Y)///AP以page为原点的
                {
                    var relativePaths = resultInk.GetPath(ink.AnnotBox.LBPoint);
                }
                else
                {
                    Assert.True((int)inkPointFirst.X == (int)(ink.AnnotBox.Left + pathFirst.X));
                    Assert.True((int)inkPointFirst.Y == (int)(ink.AnnotBox.Bottom + pathFirst.Y));
                }
            }
#elif IOS || MACCATALYST
            var ink = resultAnnot as PdfInkAnnotation;
            var resultInk = ink.InkListPaths[0];
            var resultPath = UIBezierPathExtension.GetPath(resultInk);
#endif
        }

#if ANDROID || WINDOWS
        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink)]
        public void AddAnnotation_Ink_Points_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var color = Color.Pink;
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var rect = PdfRectangleF.FromLTRB(50, 300, 300, 50);
            var newAnnot = page.AddAnnotation(subtype) as PdfInkAnnotation;
            newAnnot.AnnotBox = rect;

            var points = new List<PointF>()
            {
                rect.LTPoint,
                rect.RTPoint,
                rect.RBPoint,
                rect.LBPoint,
                rect.LTPoint
            };
            newAnnot.AddInkPoints(points);
            newAnnot.InkPointsColor = color;
            //newAnnot.Dispose();

            var data = _fixture.Save(doc);
            PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Ink_Points_Test)}_Result.pdf");

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);

            var resultPoints = (resultAnnot as PdfInkAnnotation).GetInkPoints();
            Assert.NotNull(resultPoints);
            Assert.Equal(1, resultPoints.Count);
            Assert.Equal(5, resultPoints[0].Count);
            Assert.Equal(50, resultPoints[0][4].X);
        }
#endif

#if ANDROID || WINDOWS
        //stroke color;width;cap style;path;stroke mode
        const string mytest_5_inkannotation_apStream = "0.000 0.302 0.902 RG 1.50 w  1 J 1 j 97.256050 681.571350 m 97.159363 681.329773 96.901207 680.974609 96.675926 680.121704 c 96.450645 679.268799 96.155640 677.950745 95.904358 676.453979 c 95.653084 674.957214 95.389801 673.478455 95.168243 671.140991 c 94.946678 668.803589 94.744019 665.611267 94.574982 662.429321 c 94.405945 659.247437 94.268051 655.402771 94.154015 652.049683 c 94.039970 648.696533 93.957901 645.469238 93.890755 642.310547 c 93.823593 639.151917 93.783394 635.912109 93.751099 633.097656 c 93.718803 630.283325 93.706360 627.684692 93.696983 625.424194 c 93.687607 623.163757 93.691170 621.327515 93.694847 619.534973 c 93.698524 617.742432 93.661140 616.309570 93.719055 614.669189 c 93.776985 613.028809 93.918846 611.219727 94.042389 609.692688 c 94.165932 608.165588 94.390663 606.204407 94.460320 605.506714 c S";
        const string my_apStream = "1 0 0 1 0 0 cm 0 0 255 RG 5 w 50 300 m 300 300 l 300 50 l 50 50 l h S";
        const string AddAnnotation_Ink_Path_Test_pdfium_gnenerate_apStream = "q 1 0 0 1 0 0 cm 50 300 m 50 300 m 300 300 l 300 50 l 50 50 l h n Q";
        //添加S表示stroke mode
        const string AddAnnotation_Ink_Path_Test_pdfium_gnenerate_fix_apStream = "q 1 0 0 1 0 0 cm 50 300 m 50 300 m 300 300 l 300 50 l 50 50 l h S Q";
        //pdfium无法修改bbox, 因此不能处理ios的ap
        const string AddAnnotation_Ink_Path_Test_iOS_gnenerate_apStream = "q Q q 0 0 252 252 re W n 1 j /Cs3 CS 0 0 1 SC 0 250 m 0 250 l 250 250 l 250 0 l 0 0 l h S Q";
        const string AddAnnotation_Ink_Path_Test_iOS_gnenerate_changed_apStream = "q Q q 0 0 252 252 re W n 1 j 0 0 1 SC 0 250 m 0 250 l 250 250 l 250 0 l 0 0 l h S Q";
        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, mytest_5_inkannotation_apStream, 92.6611f, 604.507f, 98.2561f, 682.571f, 1)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, my_apStream, 30f, 510f, 510f, 30f, 2)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, AddAnnotation_Ink_Path_Test_pdfium_gnenerate_apStream, 49, 301, 301, 49, 3)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, AddAnnotation_Ink_Path_Test_pdfium_gnenerate_fix_apStream, 49, 301, 301, 49, 4)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, AddAnnotation_Ink_Path_Test_iOS_gnenerate_apStream, 49, 301, 301, 49, 5)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink, AddAnnotation_Ink_Path_Test_iOS_gnenerate_changed_apStream, 49, 301, 301, 49, 6)]
        public void AddAnnotation_Ink_ByAPString_Test(string filePath, PdfAnnotationSubtype subtype, string apStream, float l, float t, float r, float b, int resultTag)
        {
            byte[] data = null;
            using (var doc = LoadPdfDocument(filePath, null))
            {
                using (var pageReader = doc.GetPage(0))
                {
                    var annot = pageReader.AddAnnotation(PdfAnnotationSubtype.Ink);
                    annot.AnnotBox = PdfRectangleF.FromLTRB(l, t, r, b);

                    var inkAnnot = annot as PdfInkAnnotation;
                    //某些软件会为/AP中的路径添加/Inklist, 记录这些点的好处可能是能产生合适的/Rect
                    inkAnnot.AddInkPoints(new List<PointF>()
                    {
                        new PointF(l, t), new PointF(l, t), new PointF(r, t), new PointF(r, b), new PointF(l, b), new PointF(l, t),
                    });
                    annot.SetFlags(PDFiumCore.FPDFAnnotationFlag.Print);
                    annot.SetBorder(0, 0, 0);
                    annot.SetAnnotColor(Color.Red);
                    inkAnnot.InkPointsColor = Color.Red;
                    var success = annot.SetAppearenceStr(apStream);

                    //pageReader.SaveNewContent()
                    var ap = pageReader.GetAnnotation(0).GetAppearenceStr();
                    Assert.Equal(apStream, ap);
                }
                data = _fixture.Save(doc);
                PdfSaveExtension.Save(_fixture, doc, $"{this.GetType().Name}_{nameof(AddAnnotation_Ink_ByAPString_Test)}_Result_{resultTag}.pdf");
            }

            var resultpage = _fixture.LoadPdfDocument(data, null).GetPage(0);

            var resultannotFirst = resultpage.GetAnnotation(0);
            var resultannot = resultannotFirst as PdfInkAnnotation;
            var resultap = resultannot.GetAppearenceStr();
            Assert.Equal(apStream, resultap);
            var objs = resultannot.PdfPageObjs;
            Assert.NotNull(objs);
            var paths = (objs[0] as PdfPagePathObj).GetPath();
        }
#endif

        [Theory]
        [InlineData("Docs/mytest/mytest_5_inkannotation.pdf", PdfAnnotationSubtype.Ink)]
        [InlineData("Docs/mytest/mytest_edit_annotation_iosdraw.pdf", PdfAnnotationSubtype.Ink)]
        [InlineData("Docs/mytest/mytest_edit_annotation_ios_rect.pdf", PdfAnnotationSubtype.Ink)]
        public void PdfInkAnnotation_Paths_Test(string filePath, PdfAnnotationSubtype type)
        {
            var doc = LoadPdfDocument(filePath, null);
            var pageReader = doc.GetPage(0);
            var annots = pageReader.GetAnnotations();
            var annot = annots[0];
            Assert.Equal(type, annot.AnnotationType);
            var inkAnnot = annot as PdfInkAnnotation;
#if ANDROID || WINDOWS
            var inkPointFirst = inkAnnot.GetInkPoints()[0][0];
            var objs = inkAnnot.PdfPageObjs;
            Assert.NotNull(objs);
            var obj = inkAnnot.PdfPageObjs[0];
            Assert.True(obj is PdfPagePathObj);
            var pathObj = obj as PdfPagePathObj;
            var paths = pathObj.GetPath();
            if (paths[0].Type == PdfSegmentPath.SegmentFlag.MoveTo)
            { 
                var pathFirst = paths[0].Position;
                if((int)pathFirst.X == (int)inkPointFirst.X && (int)pathFirst.Y == (int)inkPointFirst.Y)
                {
                    var relativePaths = pathObj.GetPath(inkAnnot.AnnotBox.LBPoint);
                }
                else
                {
                    Assert.True((int)inkPointFirst.X == (int)(inkAnnot.AnnotBox.Left + pathFirst.X));
                    Assert.True((int)inkPointFirst.Y == (int)(inkAnnot.AnnotBox.Bottom + pathFirst.Y));
                }
            }
#elif IOS || MACCATALYST
            Assert.NotNull(inkAnnot.InkListPaths);
            Assert.Equal(1, inkAnnot.InkListPaths.Length);
            var resultInk = inkAnnot.InkListPaths[0];
            var resultPath = UIBezierPathExtension.GetPath(resultInk);
#endif
        }

        #endregion

        #region Line

        [Theory]
        [InlineData("Docs/mytest/mytest_4_lineannotation.pdf", PdfAnnotationSubtype.Line)]
        public void PdfLineAnnotation_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var annots = page.GetAnnotations();
            var annot = annots[0];
            Assert.Equal(subtype, annot.AnnotationType);
            var lineAnnot = annot as PdfLineAnnotation;
            var location = lineAnnot.Location;
            Assert.NotNull(location);
        }

        #endregion
    }
}
