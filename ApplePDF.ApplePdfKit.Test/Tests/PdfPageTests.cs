using ApplePDF.PdfKit;
using System.Runtime.InteropServices;
using Xunit;
using IPdfPage = ApplePDF.PdfKit.IPdfPage;
using PointF = System.Drawing.PointF;
using Size = System.Drawing.Size;
using Color = System.Drawing.Color;
using ApplePDF.ApplePdfKit.Test.Tests;
using ApplePDF.PdfKit.Annotation;

#if IOS || MACCATALYST
using Lib = ApplePDF.PdfKit.PdfKitLib;
#else
using Lib = ApplePDF.PdfKit.PdfiumLib;
#endif
namespace ApplePDF.Test
{
    public sealed class PdfPageTests
    {
        private readonly ILib _fixture = Lib.Instance;
        string[] AllAssets = typeof(PdfPageTests).Assembly.GetManifestResourceNames();
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

        public PdfPageTests()
        {

        }

        private void ExecuteForDocument(string filePath, string password, int pageIndex, Action<IPdfPage> action)
        {
            using (var doc = LoadPdfDocument(filePath, password))
            using (var page = doc.GetPage(pageIndex) as IPdfPage)
            {
                action(page);
            }
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf")]
        public void PageIndex_WhenCalled_ShouldReturnCorrectIndex(string filePath)
        {
            var random = new System.Random();

            var index = random.Next(19);

            ExecuteForDocument(filePath, null, index, pageReader =>
             {
                 Assert.Equal(index, pageReader.PageIndex);
             });
        }

#if ANDROID || WINDOWS
        [Theory]
        [InlineData("Docs/Docnet/simple_6.pdf", "Horizontal", "Vertical")]
        public void GetCharacters_WhenCalled_ShouldReturnCorrectCharacters(string filePath, string hopeFirstWord, string hopeSecondWord)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var characters = (pageReader as PdfPage).GetCharacters().ToArray();

                Assert.Equal(20, characters.Length);

                var firstText = string.Empty;

                for (var i = 0; i < 10; i++)
                {
                    var ch = characters[i];

                    Assert.Equal(12, ch.FontSize);
                    Assert.Equal(0, ch.Angle);

                    firstText += ch.Char;
                }

                Assert.Equal(hopeFirstWord, firstText);

                var secondText = string.Empty;

                for (var i = 12; i < 20; i++)
                {
                    var ch = characters[i];

                    Assert.Equal(12, ch.FontSize);
                    Assert.Equal(4.712, ch.Angle, 3);

                    secondText += ch.Char;
                }

                Assert.Equal(hopeSecondWord, secondText);
            });
        }
#endif
        [Theory]
        [InlineData("Docs/Docnet/simple_2.pdf", 1, "2")]
        [InlineData("Docs/Docnet/simple_2.pdf", 3, "4 CONTENTS")]
        [InlineData("Docs/Docnet/simple_4.pdf", 0, "")]
        [InlineData("Docs/Docnet/simple_5.pdf", 0, "test.md 11/11/2018\r\n1 / 1\r\nTest document")]
        public void GetText_WhenCalled_ShouldReturnCorrectText(string filePath, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, null, pageIndex, pageReader =>
            {
                var text = pageReader.Text;

                Assert.Equal(expectedText, text);
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1, "Simple PDF File 2")]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1, "Boring. More,")]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1, "The end, and just as well.")]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 4, "ASCIIHexDecode")]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 0, "The Secret (2016 film)")]
        public void GetText_WhenCalled_ShouldContainCorrectText(string filePath, string password, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
          {
              var text = pageReader.Text;
              var c = text.Contains(expectedText, StringComparison.Ordinal);
              Assert.Equal(true, c);
          });
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_chinese.pdf", null, 0, "另一端")]
        public void GetText_Chinese_WhenCalled_ShouldContainCorrectText(string filePath, string password, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var text = pageReader.Text;
                var c = text.Contains(expectedText, StringComparison.Ordinal);
                Assert.Equal(true, c);
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_2.pdf", null, 1, 1)]
        [InlineData("Docs/Docnet/simple_2.pdf", null, 3, 10)]
        [InlineData("Docs/Docnet/simple_5.pdf", null, 0, 40)]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 0, 2009)]
        public void GetCharacters_WhenCalled_ShouldReturnCorrectCharactersLength(string filePath, string password, int pageIndex, int charCount)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var characters = pageReader.Text.ToArray();

                Assert.Equal(charCount, characters.Length);
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1)]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 18)]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 0)]
        [InlineData("Docs/Docnet/simple_3.pdf", null, 1)]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 18)]
        [InlineData("Docs/Docnet/protected_0.pdf", "password", 0)]
        public void DrawTest(string filePath, string password, int pageIndex)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var size = pageReader.GetSize();
                byte[] data = new byte[(int)size.Width * (int)size.Height * 4];
                GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                pageReader.Draw(pointer, (int)size.Width, false);
                Assert.True(data.Any(t => (int)t > 1));
                pinnedArray.Free();
            });
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_chinese.pdf", "thumbnail.png")]
        public void GetThumbilTest(string filePath, string thumbnailFilePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var pageSize = pageReader.GetSize();
                var imageObj = pageReader.GetThumbnail(new Size((int)pageSize.Width, (int)pageSize.Height), PdfDisplayBox.Media);
            });
        }

        [Theory]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 1, 595, 841)]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 10, 5953, 8419)]
        [InlineData("Docs/Docnet/simple_0.pdf", null, 15, 8929, 12628)]
        public void GetSize_WhenCalledWithScalingFactor_ShouldMatch(string filePath, string password, double scaling, int expectedWidth, int expectedHeight)
        {
            ExecuteForDocument(filePath, password, 0, (pageReader =>
           {
               var rect = pageReader.GetSize();
               var width = rect.Width * scaling;
               var height = rect.Height * scaling;

               Assert.Equal(expectedWidth, width, 1);
               Assert.Equal(expectedHeight, height, 1);
           }));
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_4_highlightannotation.pdf", 4)]
        [InlineData("Docs/mytest/mytest_5_inkannotation.pdf", 5)]
        [InlineData("Docs/mytest/mytest_4_freetextannotation.pdf", 4)]
        [InlineData("Docs/mytest/mytest_4_rectangleannotation.pdf", 4)]
        [InlineData("Docs/mytest/mytest_4_linkannotation.pdf", 4)]
        public void Annotations_WhenCalled_ShouldGetCurrectAnnotationsCount(string filePath, int annotationsCount)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.Equal(annotationsCount, pageReader.AnnotationCount);
            });
        }

#if ANDROID || WINDOWS
        /// <summary>
        /// Error:Any char instead will be \ufffe
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", "little", "123456")]
        public void InsteadText_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string oldText, string newText)
        {
            byte[] data = null;
            var orininalDoc = LoadPdfDocument(filePath, null);
            var pageReader = orininalDoc.GetPage(0);
            pageReader.InsteadText(oldText, newText);
            var success = pageReader.SaveNewContent();
            var text = pageReader.Text;
            var doc = pageReader.Document;
            pageReader.Dispose();
            var newPage = doc.GetPage(0);
            text = newPage.Text;
            Assert.True(text.Contains(newText));
            data = _fixture.Save(doc);
            //是否存入新文档
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultText = resultDoc.GetPage(0).Text;
            Assert.True(resultText.Contains(newText));
        }
#endif

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", "Helvetica", 12, "0123456789abcdABCD-+/.<>?@!#%*", 200, 200, 2)]
        public void AddText_UseStandardFont_Test(string filePath, string fontName, float fontSize, string addText, double x, double y, double scale)
        {
            byte[] data = null;
            var originalDoc = LoadPdfDocument(filePath, null);
            var pageReader = originalDoc.GetPage(0);
            PdfFont font = new PdfFont(pageReader.Document, fontName);
            pageReader.AddText(font, fontSize, Color.Red, addText, x, y, scale);
#if ANDROID || WINDOWS
            var success = (pageReader as PdfPage).SaveNewContent();
#endif
            var text = pageReader.Text;
            Assert.True(text?.Contains(addText));
            pageReader.Dispose();
            data = _fixture.Save(originalDoc);
            PdfSaveExtension.Save(_fixture, originalDoc, $"{nameof(PdfPageTests)}_{nameof(AddText_UseStandardFont_Test)}_Result.pdf");
            //看是否保存
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultText = resultDoc.GetPage(0).Text;
            Assert.True(resultText?.Contains(addText));
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", "Fonts/YouYuan.ttf", 12, "0123456789你好abcdABCD-+/.<>?@!#%*你好", 200, 200, 2)]
        public void AddText_UseCustomFont_Test(string filePath, string customFontPath, float fontSize, string addText, double x, double y, double scale)
        {
            byte[] data = null;
            var originalDoc = LoadPdfDocument(filePath, null);
            var pageReader = originalDoc.GetPage(0);
            var fontData = LoadFont(customFontPath);
            PdfFont font = new PdfFont(originalDoc, fontData);
            pageReader.AddText(font, fontSize, Color.Red, addText, x, y, scale);
#if ANDROID || WINDOWS
            var success = pageReader.SaveNewContent();
#endif
            var text = pageReader.Text;
            var doc = pageReader.Document;
            pageReader.Dispose();
            data = _fixture.Save(doc);
#if WINDOWS || MACCATALYST
            _fixture.Save(doc, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{nameof(PdfPageTests)}_{nameof(AddText_UseCustomFont_Test)}_Result.pdf"));
#endif
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultText = resultDoc.GetPage(0).Text;
            Assert.True(resultText.Contains(addText));
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InTwoPoint_ShouldReturnTextInPoints(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(new System.Drawing.PointF(x1, y1), new System.Drawing.PointF(x2, y2));
                Assert.Equal(text, selection.Text);
            });
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InRect_ShouldReturnTextInRect(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(PdfRectangleF.FromLTRB(x1, y1, x2, y2));
                Assert.Equal(text, selection.Text);
            });
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 143, 782, "Another paragraph, this time a little bit longer to make sure, this line will be divided into at least")]
        public void SelectLine_ShouldReturnTextInLine(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.SelectLine(new PointF(x1, y1));
                var actual = selection.Text;
                Assert.True(actual.Contains(text));//使用Contains，因为返回值可能有换行符号
            });
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 73, 795, "p")]
        public void SelectWord_ShouldReturnWordAtPosition(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.SelectWord(new PointF(x1, y1));
                var actual = selection.Text;
                Assert.True(actual.Contains(text));//使用Contains，因为返回值可能有换行符号
            });
        }

#if ANDROID || WINDOWS
        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        public void GetCharactersBounds_ShouldReturnBoundsOfText(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                //获得文字的矩形区域
                var actual = (pageReader as PdfPage).GetCharactersBounds(0, text.Length);
                Assert.True(actual.Length > 0);
                Assert.True(actual[0].IsContainPoint(new PointF(x1, y1)));
                //选择矩形区域
                var selection0 = pageReader.GetSelection(actual[0]);
                Assert.True(selection0.Text.Contains("First"));
                var selection1 = pageReader.GetSelection(actual[1]);
                Assert.True(selection1.Text.Contains("paragraph"));
                var selections = selection0.Clone() as PdfSelection;
                selections.AddSelection(selection1);
                Assert.True(selections.Text.Contains("First paragraph"));
                var attrStrs = selections.AttributedString;
                foreach (var attr in attrStrs)
                {
                    attr.Page = pageReader as PdfPage;
                    var fontSize = attr.FontSize;
                    var strokeColor = attr.StrokeColor;
                    Assert.True(strokeColor.A == Color.Black.A &&
                        strokeColor.R == Color.Black.R &&
                        strokeColor.G == Color.Black.G &&
                        strokeColor.B == Color.Black.B);
                    var fillColor = attr.FillColor;
                    var fontName = attr.FontName;
                    var fontWeight = attr.FontWeight;
                    var angle = attr.Angle;
                }
            });
        }
#endif

#if ANDROID || WINDOWS
        [SkippableTheory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", 73, 795)]
        public void AppendObj_Path_Test(string filePath, int x1, int y1)
        {
            var originalDoc = LoadPdfDocument(filePath, null);
            var pageReader = originalDoc.GetPage(0);
            var selection = pageReader.SelectWord(new PointF(x1, y1));
            var rectPathObj = PdfPagePathObj.Create(new PointF(100, 100));
            rectPathObj.AddPath(PdfSegmentPath.GenerateRectSegments(300, 300, 100, 200));
            rectPathObj.AddPath(PdfSegmentPath.GenerateRoundRectSegments(250, 350, 100, 200, 20));
            rectPathObj.SetStrokeColor(Color.HotPink);
            rectPathObj.SetFillColor(Color.Gray);
            rectPathObj.SetDrawMode(true);
            var circlPpathObj = PdfPagePathObj.Create(new PointF(100, 100));
            circlPpathObj.SetDrawMode(true);
            circlPpathObj.AddPath(PdfSegmentPath.GenerateTriangleSegments(new PointF(100, 100), new PointF(100, 400), new PointF(200, 400)));
            circlPpathObj.AddPath(PdfSegmentPath.GenerateCircleSegments(100, 300, 300));
            circlPpathObj.SetFillColor(Color.Red);
            circlPpathObj.SetStrokeColor(Color.Blue);
            var arcPathObj = PdfPagePathObj.Create(new PointF(100, 100));
            arcPathObj.SetDrawMode(true);
            arcPathObj.AddPath(PdfSegmentPath.GenerateArcSegments(300, 300, 100, 300 + -360, 180));
            arcPathObj.SetStrokeColor(Color.Green);
            arcPathObj.SetStrokeWidth(5);
            pageReader.AppendObj(rectPathObj);
            pageReader.AppendObj(circlPpathObj);
            pageReader.AppendObj(arcPathObj);
            pageReader.SaveNewContent();
            byte[] data = _fixture.Save(pageReader.Document);
#if WINDOWS
            _fixture.Save(pageReader.Document, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{nameof(PdfPageTests)}_{nameof(AppendObj_Path_Test)}_Result.pdf"));
#endif
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var objectCount = resultPage.GetObjCount();
            if (objectCount > 0)
            {
                var pdfPageObjs = resultPage.GetAllObj();
                foreach (var obj in pdfPageObjs)
                {
                    switch (obj.Type)
                    {
                        case PdfPageObj.TypeFlag.Unknow:
                            break;
                        case PdfPageObj.TypeFlag.Text:
                            break;
                        case PdfPageObj.TypeFlag.Path:
                            var pathObj = obj as PdfPagePathObj;
                            var fillColor = pathObj.GetFillColor();
                            var strokeColor = pathObj.GetStrokeColor();
                            var path = pathObj.GetPath();
                            break;
                        case PdfPageObj.TypeFlag.Image:
                            break;
                        case PdfPageObj.TypeFlag.Shading:
                            break;
                        case PdfPageObj.TypeFlag.Form:
                            break;
                    }
                }
            }

            Skip.If(true, "请Debug和查看结果检查是否绘制正确");
        }
#endif

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Circle)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.FreeText)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Highlight)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Ink)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Link)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Square)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Squiggly)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Stamp)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.StrikeOut)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Text)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Underline)]
        public void AddAnnotation_Support_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var newAnnot = page.AddAnnotation(subtype);
            Assert.Equal(subtype, newAnnot.AnnotationType);
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
        }
        
        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Highlight, PdfAnnotationSubtype.Popup)]
        public void AddAnnotation_Support_Popup_Test(string filePath, PdfAnnotationSubtype parentSubtype, PdfAnnotationSubtype subtype)
        {
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
            var newAnnot = page.AddAnnotation(parentSubtype);
            Assert.Equal(parentSubtype, newAnnot.AnnotationType);
            var parentAnnot = newAnnot as PdfHighlightAnnotation;
            var popupAnnot = parentAnnot.AddPopupAnnotation();
            popupAnnot.Text = "有Popup";
#if ANDROID || WINDOWS
            page.SaveNewContent();
#endif
            var data = _fixture.Save(doc);
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnot = resultPage.GetAnnotations()[0];
            Assert.Equal(parentSubtype, resultAnnot.AnnotationType);
            Assert.Equal(subtype, (resultAnnot as PdfHighlightAnnotation).PopupAnnotation.AnnotationType);
        }

        [Theory]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Line)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Widget)]
        [InlineData("Docs/mytest/mytest_edit_annotation.pdf", PdfAnnotationSubtype.Unknow)]
        public void AddAnnotation_iOSSupport_PdfiumNotSupport_Test(string filePath, PdfAnnotationSubtype subtype)
        {
            var doc = LoadPdfDocument(filePath, null);
            var page = doc.GetPage(0);
#if ANDROID || WINDOWS
            Assert.Throws<NotSupportedException>(() =>
            {
                var newAnnot = page.AddAnnotation(subtype);
            });
#else
            var newAnnot = page.AddAnnotation(subtype);
            Assert.Equal(subtype, newAnnot.AnnotationType);

            var data = _fixture.Save(doc);

            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultPage = resultDoc.GetPage(0);
            var resultAnnots = resultPage.GetAnnotations();
            var resultAnnot = resultAnnots[0];
            Assert.Equal(subtype, resultAnnot.AnnotationType);
#endif
        }
    }
}
