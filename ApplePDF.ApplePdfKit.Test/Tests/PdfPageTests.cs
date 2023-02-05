using ApplePDF.PdfKit;
using System.Runtime.InteropServices;
using Xunit;
using IPdfPage = ApplePDF.PdfKit.IPdfPage;
using PointF = System.Drawing.PointF;
using Size = System.Drawing.Size;
using Color = System.Drawing.Color;

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
        PdfDocument LoadPdfDocument(string path, string password)
        {
            //原资源名 Docs/Docnet/simple_0.pdf
            //嵌入后的资源名 ApplePDF.ApplePdfKit.Test.Docnet.simple_0.pdf
            if (path.Contains("Docs/"))
            {
                path = path.Replace("Docs/", "");
            }
            path = path.Replace('/', '.');
            var fileName = "ApplePDF.ApplePdfKit.Test." + path;
            return _fixture.LoadPdfDocument(ReadEmbedAssetBytes(fileName), password);
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
        [InlineData("Docs/mytest_chinese.pdf", null, 0, "另一端")]
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

        [InlineData("Docs/mytest_chinese.pdf", "thumbnail.png")]
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
        [InlineData("Docs/mytest_4_highlightannotation.pdf", 4)]
        [InlineData("Docs/mytest_5_inkannotation.pdf", 5)]
        [InlineData("Docs/mytest_4_freetextannotation.pdf", 4)]
        [InlineData("Docs/mytest_4_rectangleannotation.pdf", 4)]
        [InlineData("Docs/mytest_4_linkannotation.pdf", 4)]
        public void Annotations_WhenCalled_ShouldGetCurrectAnnotationsCount(string filePath, int annotationsCount)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.Equal(annotationsCount, pageReader.AnnotationCount);
            });
        }

        [InlineData("Docs/mytest_chinese.pdf", "这是一个中文注释")]
        public void Annotations_Chinese_WhenCalled_ShouldGetCurrectTextOfPopup(string filePath, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
#if ANDROID || WINDOWS
                var isContain = (pageReader.GetAnnotations()[0] as PdfKit.Annotation.PdfHighlightAnnotation).PopupAnnotation.Text.Contains(text);
                Assert.Equal(true, isContain);
#endif
            });
        }

#if ANDROID || WINDOWS
        /// <summary>
        /// Error:Any char instead will be \ufffe
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        [InlineData("Docs/mytest_edit_annotation.pdf", "little", "123456")]
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

        [InlineData("Docs/mytest_edit_annotation.pdf", "Helvetica", 12, "0123456789abcdABCD-+/.<>?@!#%*", 200, 200, 2)]
        public void AddText_UseStandardFont_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string fontName, float fontSize, string addText, double x, double y, double scale)
        {
            byte[] data = null;
            var originalDoc = LoadPdfDocument(filePath, null);
            var pageReader = originalDoc.GetPage(0);
            PdfFont font = new PdfFont(pageReader.Document, fontName);
            pageReader.AddText(font, fontSize, addText, x, y, scale);
#if ANDROID || WINDOWS
            var success = (pageReader as PdfPage).SaveNewContent();
#endif
            var text = pageReader.Text;
            Assert.True(text.Contains(addText));
            pageReader.Dispose();
            data = _fixture.Save(originalDoc);
            //看是否保存
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultText = resultDoc.GetPage(0).Text;
            Assert.True(resultText.Contains(addText));
        }

        [InlineData("Docs/mytest_edit_annotation.pdf", "Fonts/YouYuan.ttf", 12, "0123456789你好abcdABCD-+/.<>?@!#%*你好", 200, 200, 2)]
        public void AddText_UseCustomFont_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string customFontPath, float fontSize, string addText, double x, double y, double scale)
        {
            byte[] data = null;
            var originalDoc = LoadPdfDocument(filePath, null);
            var pageReader = originalDoc.GetPage(0);
            var fontData = ReadEmbedAssetBytes(customFontPath);
            PdfFont font = new PdfFont(originalDoc, fontData);
            pageReader.AddText(font, fontSize, addText, x, y, scale);
#if ANDROID || WINDOWS
            var success = pageReader.SaveNewContent();
#endif
            var text = pageReader.Text;
            var doc = pageReader.Document;
            pageReader.Dispose();
            data = _fixture.Save(doc);
            var resultDoc = _fixture.LoadPdfDocument(data, null);
            var resultText = resultDoc.GetPage(0).Text;
            Assert.True(resultText.Contains(addText));
        }

        [Theory]
        [InlineData("Docs/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InTwoPoint_ShouldReturnTextInPoints(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(new System.Drawing.PointF(x1, y1), new System.Drawing.PointF(x2, y2));
                Assert.Equal(text, selection.Text);
            });
        }

        [Theory]
        [InlineData("Docs/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InRect_ShouldReturnTextInRect(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(PdfRectangleF.FromLTRB(x1, y1, x2, y2));
                Assert.Equal(text, selection.Text);
            });
        }

        [Theory]
        [InlineData("Docs/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        [InlineData("Docs/mytest_edit_annotation.pdf", 143, 782, "Another paragraph, this time a little bit longer to make sure, this line will be divided into at least")]
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
        [InlineData("Docs/mytest_edit_annotation.pdf", 73, 795, "p")]
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
        [InlineData("Docs/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        public void GetCharactersBounds_ShouldReturnBoundsOfText(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                //获得文字的矩形区域
                var actual = (pageReader as PdfPage).GetCharactersBounds(0, text.Length);
                Assert.True(actual.Count > 0);
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
        /*        [InlineData("Docs/mytest_edit_annotation.pdf", 73, 795, "p")]
                public void DrawPath_ShouldReturnWordAtPosition(string filePath, int x1, int y1, string text)
                {
                    ExecuteForDocument(filePath, null, 0, pageReader =>
                    {
                        var selection = pageReader.SelectWord(new PointF(x1, y1));
                        var rectPathObj = PdfPagePathObj.Create(new PointF(100, 100));
                        rectPathObj.AddPath(PdfSegmentPath.GenerateRectSegments(300, 300, 100, 200));
                        rectPathObj.AddPath(PdfSegmentPath.GenerateRoundRectSegments(250, 350, 100, 200, 20));
                        rectPathObj.SetStrokeColor(Color.HotPink);
                        rectPathObj.SetFillColor(Color.Gray);
                        rectPathObj.SetDrawMode(true);
                        var circlPpathObj = PdfPagePathObj.Create(new PointF(100, 100));
                        circlPpathObj.SetDrawMode(true);
                        circlPpathObj.AddPath(PdfSegmentPath.GenerateTriangleSegments(new PointF(100,100), new PointF(100,400),new PointF(200,400)));
                        circlPpathObj.AddPath(PdfSegmentPath.GenerateCircleSegments(100, 300, 300));
                        circlPpathObj.SetFillColor(Color.Red);
                        circlPpathObj.SetStrokeColor(Color.Blue);
                        var arcPathObj = PdfPagePathObj.Create(new PointF(100, 100));
                        arcPathObj.SetDrawMode(true);
                        arcPathObj.AddPath(PdfSegmentPath.GenerateArcSegments(300, 300, 100, 300 + -360, 180 ));
                        arcPathObj.SetStrokeColor(Color.Green);
                        arcPathObj.SetStrokeWidth(5);
                        pageReader.AppendObj(rectPathObj);
                        pageReader.AppendObj(circlPpathObj);
                        pageReader.AppendObj(arcPathObj);
                        pageReader.SaveNewContent();
                        PdfiumLib.Instance.Save(pageReader.Document, "Result.pdf");
                    });
                    ExecuteForDocument("Result.pdf", null, 0, pageReader =>
                    {
                        var objectCount = fpdf_edit.FPDFPageCountObjects(pageReader.Page);
                        if (objectCount > 0)
                        {
                            var pdfPageObjs = new List<PdfPageObj>();
                            //此处分析注释数据时只当注释只有一个文本和图像对象
                            for (int objIndex = 0; objIndex < objectCount; objIndex++)
                            {
                                var obj = fpdf_edit.FPDFPageGetObject(pageReader.Page, objIndex);
                                if (obj != null)
                                {
                                    var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                                    if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                                    {
                                        pdfPageObjs.Add(new PdfPageTextObj(obj));
                                    }
                                    else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                                    {
                                        pdfPageObjs.Add(new PdfPageImageObj(obj));
                                    }
                                    else if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                                    {
                                        pdfPageObjs.Add(new PdfPagePathObj(obj));
                                    }
                                }
                            }
                            foreach (var obj in pdfPageObjs)
                            {
                                switch (obj.Type)
                                {
                                    case PdfPageObjectTypeFlag.UNKNOWN:
                                        break;
                                    case PdfPageObjectTypeFlag.TEXT:
                                        break;
                                    case PdfPageObjectTypeFlag.PATH:
                                        var pathObj = obj as PdfPagePathObj;
                                        var fillColor = pathObj.GetFillColor();
                                        var strokeColor = pathObj.GetStrokeColor();
                                        var path = pathObj.GetPath();
                                        break;
                                    case PdfPageObjectTypeFlag.IMAGE:
                                        break;
                                    case PdfPageObjectTypeFlag.SHADING:
                                        break;
                                    case PdfPageObjectTypeFlag.FORM:
                                        break;
                                }
                            }
                        }
                    });
                }
        */
    }
}
