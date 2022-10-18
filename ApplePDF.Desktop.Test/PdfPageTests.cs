using ApplePDF.PdfKit;
using ApplePDF.PdfKit.Annotation;
using NUnit.Framework;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ApplePDF.Test
{
    [TestFixture]
    public sealed class PdfPageTests
    {
        private readonly Pdfium _fixture = Pdfium.Instance;

        public PdfPageTests()
        {

        }

        private void ExecuteForDocument(string filePath, string password, int pageIndex, Action<PdfPage> action)
        {
            using (var doc = _fixture.LoadPdfDocument(filePath, password))
            using (var page = doc.GetPage(pageIndex))
            {
                action(page);
            }
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_0.pdf")]
        public void PageIndex_WhenCalled_ShouldReturnCorrectIndex(string filePath)
        {
            var random = new Random();

            var index = random.Next(19);

            ExecuteForDocument(filePath, null, index, pageReader =>
             {
                 Assert.AreEqual(index, pageReader.PageIndex);
             });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_6.pdf", "Horizontal", "Vertical")]
        public void GetCharacters_WhenCalled_ShouldReturnCorrectCharacters(string filePath, string hopeFirstWord, string hopeSecondWord)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var characters = pageReader.GetCharacters().ToArray();

                Assert.AreEqual(20, characters.Length);

                var firstText = string.Empty;

                for (var i = 0; i < 10; i++)
                {
                    var ch = characters[i];

                    Assert.AreEqual(12, ch.FontSize);
                    Assert.AreEqual(0, ch.Angle);

                    firstText += ch.Char;
                }

                Assert.AreEqual(hopeFirstWord, firstText);

                var secondText = string.Empty;

                for (var i = 12; i < 20; i++)
                {
                    var ch = characters[i];

                    Assert.AreEqual(12, ch.FontSize);
                    Assert.AreEqual(4.712, ch.Angle, 3);

                    secondText += ch.Char;
                }

                Assert.AreEqual(hopeSecondWord, secondText);
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_2.pdf", 1, "2")]
        [TestCase("Docs/Docnet/simple_2.pdf", 3, "4 CONTENTS")]
        [TestCase("Docs/Docnet/simple_4.pdf", 0, "")]
        [TestCase("Docs/Docnet/simple_5.pdf", 0, "test.md 11/11/2018\r\n1 / 1\r\nTest document")]
        [TestCase("Docs/Docnet/simple_2.pdf", 1, "2")]
        [TestCase("Docs/Docnet/simple_2.pdf", 3, "4 CONTENTS")]
        [TestCase("Docs/Docnet/simple_4.pdf", 0, "")]
        [TestCase("Docs/Docnet/simple_5.pdf", 0, "test.md 11/11/2018\r\n1 / 1\r\nTest document")]
        public void GetText_WhenCalled_ShouldReturnCorrectText(string filePath, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, null, pageIndex, pageReader =>
            {
                var text = pageReader.Text;

                Assert.AreEqual(expectedText, text);
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "Simple PDF File 2")]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "Boring. More,")]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "The end, and just as well.")]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 4, "ASCIIHexDecode")]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0, "The Secret (2016 film)")]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "Simple PDF File 2")]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "Boring. More,")]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1, "The end, and just as well.")]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 4, "ASCIIHexDecode")]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0, "The Secret (2016 film)")]
        public void GetText_WhenCalled_ShouldContainCorrectText(string filePath, string password, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
          {
              var text = pageReader.Text;
              var c = text.Contains(expectedText, StringComparison.Ordinal);
              Assert.AreEqual(true, c);
          });
        }

        [TestCase("Docs/mytest_chinese.pdf", null, 0, "另一端")]
        public void GetText_Chinese_WhenCalled_ShouldContainCorrectText(string filePath, string password, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var text = pageReader.Text;
                var c = text.Contains(expectedText, StringComparison.Ordinal);
                Assert.AreEqual(true, c);
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 1, 1)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 3, 10)]
        [TestCase("Docs/Docnet/simple_5.pdf", null, 0, 40)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0, 2009)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 1, 1)]
        [TestCase("Docs/Docnet/simple_2.pdf", null, 3, 10)]
        [TestCase("Docs/Docnet/simple_5.pdf", null, 0, 40)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0, 2009)]
        public void GetCharacters_WhenCalled_ShouldReturnCorrectCharactersLength(string filePath, string password, int pageIndex, int charCount)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var characters = pageReader.GetCharacters().ToArray();

                Assert.AreEqual(charCount, characters.Length);
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 18)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 18)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0)]
        public void GetImage_WhenCalled_ShouldReturnNonZeroRawByteArray(string filePath, string password, int pageIndex)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var bytes = pageReader.Draw(1f, 1f, 0).ToArray();

                Assert.True(bytes.Length > 0);
                Assert.IsNotEmpty(bytes.Where(x => x != 0));
            });
        }

        [TestCase("Docs/mytest_chinese.pdf", "thumbnail.png")]
        public void GetThumbil_WhenCalled_ShouldReturnNonZeroRawByteArray(string filePath, string thumbnailFilePath)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var pageSize = pageReader.GetSize();
                var bytes = pageReader.GetThumbnail(new Size((int)pageSize.Width, (int)pageSize.Height), PdfDisplayBox.Media);
                var data = bytes as byte[];
                var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                using (var memoryStream = new MemoryStream(data))
                using (var fileStream = new FileStream(thumbnailFilePath, FileMode.OpenOrCreate))
                {
                    memoryStream.CopyTo(fileStream);
                    //fileStream.Write(data, 0, data.Length);
                }
                Assert.Ignore();
            });
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 18)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0)]
        [TestCase("Docs/Docnet/simple_3.pdf", null, 1)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 18)]
        [TestCase("Docs/Docnet/protected_0.pdf", "password", 0)]
        public void GetImageWithTransparentConverter_WhenCalled_ShouldReturnNonZeroRawByteArray(string filePath, string password, int pageIndex)
        {
            //ExecuteForDocument( filePath, password,  pageIndex, pageReader =>
            //{
            //    var bytes = pageReader.GetImage(new NaiveTransparencyRemover()).ToArray();

            //    Assert.True(bytes.Length > 0);
            //    Assert.NotEmpty(bytes.Where(x => x != 0));
            //});
            Assert.Ignore();
        }

        [Theory]
        public void Reader_WhenCalledFromDifferentThreads_ShouldBeAbleToHandle()
        {
            var task1 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/Docnet/simple_0.pdf", _fixture), 2000000, 2400000));
            var task2 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/Docnet/simple_1.pdf", _fixture), 190000, 200000));
            var task3 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/Docnet/simple_2.pdf", _fixture), 4500, 4900));
            var task4 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/Docnet/simple_3.pdf", _fixture), 20000, 22000));
            var task5 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/Docnet/simple_4.pdf", _fixture), 0, 0));

            Task.WaitAll(task1, task2, task3, task4, task5);
        }

        void InRange(int actual, int small, int big)
        {
            Assert.GreaterOrEqual(actual, small);
            Assert.LessOrEqual(actual, big);
        }

        [Theory]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 1, 595, 841)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 10, 5953, 8419)]
        [TestCase("Docs/Docnet/simple_0.pdf", null, 15, 8929, 12628)]
        public void GetSize_WhenCalledWithScalingFactor_ShouldMatch(string filePath, string password, double scaling, int expectedWidth, int expectedHeight)
        {
            ExecuteForDocument(filePath, password, 0, (Action<PdfPage>)(pageReader =>
           {
               var rect = pageReader.GetSize();
               var width = rect.Width * scaling;
               var height = rect.Height * scaling;

               Assert.AreEqual(expectedWidth, width, 1);
               Assert.AreEqual(expectedHeight, height, 1);
           }));
        }

        [Test]
        public void GetImage_WhenCalledWithoutRenderAnnotationsFlag_ShouldNotRenderAnnotation()
        {
            ExecuteForDocument("Docs/Docnet/annotation_0.pdf", null, 0, pageReader =>
           {
               var bytes = pageReader.Draw(1, 1, 0).ToArray();
               Assert.True(bytes.All(x => x == 0));
           });
        }

        [Test]
        public void GetImage_WhenCalledWithRenderAnnotationsFlag_ShouldRenderAnnotation()
        {
            ExecuteForDocument("Docs/Docnet/annotation_0.pdf", null, 0, pageReader =>
           {
               // verify pixel in center of image is the correct yellow color
               var bytes = pageReader.Draw(1, 1, (int)RenderFlags.RenderAnnotations).ToArray();
               const int bpp = 4;
               var center = bytes.Length / bpp / 2 * bpp; // note integer division by 2 here.  we're getting the first byte in the central pixel
               Assert.AreEqual(133, bytes[center]); // Blue
               Assert.AreEqual(244, bytes[center + 1]); // Green
               Assert.AreEqual(252, bytes[center + 2]); // Red
               Assert.AreEqual(255, bytes[center + 3]); // Alpha
           });
        }

        [Test]
        public void GetImage_WhenCalledWithRenderAnnotationsAndGrayscaleFlags_ShouldRenderAnnotationGrayscale()
        {
            ExecuteForDocument("Docs/Docnet/annotation_0.pdf", null, 0, pageReader =>
           {
               // verify pixel in center of image is the correct gray color
               var bytes = pageReader.Draw(1, 1, (int)(RenderFlags.RenderAnnotations | RenderFlags.Grayscale)).ToArray();
               const int bpp = 4;
               var center = bytes.Length / bpp / 2 * bpp; // note integer division by 2 here. we're getting the first byte in the central pixel
               Assert.AreEqual(234, bytes[center]); // Blue
               Assert.AreEqual(234, bytes[center + 1]); // Green
               Assert.AreEqual(234, bytes[center + 2]); // Red
               Assert.AreEqual(255, bytes[center + 3]); // Alpha

           });
        }

        private static int GetNonZeroByteCount(string filePath, Pdfium fixture)
        {
            var DimOne = 1000;
            var DimTwo = 1000;
            using (var reader = fixture.LoadPdfDocument(filePath, null))
            {
                using (var pageReader = reader.GetPage(0))
                {
                    var rect = pageReader.GetSize();
                    var width = rect.Width;
                    var height = rect.Height;

                    var scaleOne = DimOne / Math.Min(width, height);
                    var scalingTwo = DimTwo / Math.Max(width, height);

                    var scale = Math.Min(scaleOne, scalingTwo);
                    //return pageReader.GetImage().Count(x => x != 0);
                    return pageReader.Draw(scale, scale, 0).Count(x => x != 0);
                }
            }
        }

        [TestCase("Docs/mytest_4_highlightannotation.pdf", 4)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", 5)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", 4)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", 4)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", 4)]
        public void Annotations_WhenCalled_ShouldGetCurrectAnnotationsCount(string filePath, int annotationsCount)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                Assert.AreEqual(annotationsCount, pageReader.AnnotationCount);
            });
        }

        [TestCase("Docs/mytest_chinese.pdf", "这是一个中文注释")]
        public void Annotations_Chinese_WhenCalled_ShouldGetCurrectTextOfPopup(string filePath, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var isContain = (pageReader.GetAnnotation(0) as PdfHighlightAnnotation).PopupAnnotation.Text.Contains(text);
                Assert.AreEqual(true, isContain);
            });
        }

        /// <summary>
        /// Error:Any char instead will be \ufffe
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        [TestCase("Docs/mytest_edit_annotation.pdf", "little", "123456")]
        public void InsteadText_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string oldText, string newText)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                pageReader.InsteadText(oldText, newText);
                var success = pageReader.SaveNewContent();
                var text = pageReader.Text;
                var doc = pageReader.Document;
                pageReader.Dispose();
                var newPage = doc.GetPage(0);
                text = newPage.Text;
                Assert.IsTrue(text.Contains(newText));
                if (File.Exists("Result.pdf"))
                    File.Delete("Result.pdf");
                Pdfium.Instance.Save(doc, "Result.pdf");
            });

            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var text = pageReader.Text;
                var success = text.Contains(newText);
                Assert.IsTrue(success);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", "Helvetica", 12, "0123456789abcdABCD-+/.<>?@!#%*", 200, 200, 2)]
        public void AddText_UseStandardFont_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string fontName, float fontSize, string addText, double x, double y, double scale)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                PdfFont font = new PdfFont(pageReader.Document, fontName);
                pageReader.AddText(font, fontSize, addText, x, y, scale);
                var success = pageReader.SaveNewContent();
                var text = pageReader.Text;
                var doc = pageReader.Document;
                pageReader.Dispose();
                if (File.Exists("Result.pdf"))
                    File.Delete("Result.pdf");
                Pdfium.Instance.Save(doc, "Result.pdf");
            });

            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var text = pageReader.Text;
                var success = text.Contains(addText);
                Assert.IsTrue(success);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", "Fonts/YouYuan.ttf", 12, "0123456789你好abcdABCD-+/.<>?@!#%*你好", 200, 200, 2)]
        public void AddText_UseCustomFont_WhenCalled_ShouldResultPdfHaveNewText(string filePath, string customFontPath, float fontSize, string addText, double x, double y, double scale)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var fontData = File.ReadAllBytes(customFontPath);
                PdfFont font = new PdfFont(pageReader.Document, fontData, PdfFontType.FPDF_FONT_TRUETYPE);
                pageReader.AddText(font, fontSize, addText, x, y, scale);
                var success = pageReader.SaveNewContent();
                var text = pageReader.Text;
                var doc = pageReader.Document;
                pageReader.Dispose();
                if (File.Exists("Result.pdf"))
                    File.Delete("Result.pdf");
                Pdfium.Instance.Save(doc, "Result.pdf");
            });

            ExecuteForDocument("Result.pdf", null, 0, pageReader =>
            {
                var text = pageReader.Text;
                var success = text.Contains(addText);
                Assert.IsTrue(success);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InTwoPoint_ShouldReturnTextInPoints(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(new System.Drawing.PointF(x1, y1), new System.Drawing.PointF(x2, y2));
                Assert.AreEqual(text, selection.Text);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 40, 805, 126, 787, "First paragraph")]
        public void GetSelection_InRect_ShouldReturnTextInRect(string filePath, int x1, int y1, int x2, int y2, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.GetSelection(PdfRectangleF.FromLTRB(x1, y1, x2, y2));
                Assert.AreEqual(text, selection.Text);
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        [TestCase("Docs/mytest_edit_annotation.pdf", 143, 782, "Another paragraph, this time a little bit longer to make sure, this line will be divided into at least")]
        public void SelectLine_ShouldReturnTextInLine(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.SelectLine(new PointF(x1, y1));
                var actual = selection.Text;
                Assert.IsTrue(actual.Contains(text));//使用Contains，因为返回值可能有换行符号
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 73, 795, "p")]
        public void SelectWord_ShouldReturnWordAtPosition(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var selection = pageReader.SelectWord(new PointF(x1, y1));
                var actual = selection.Text;
                Assert.IsTrue(actual.Contains(text));//使用Contains，因为返回值可能有换行符号
            });
        }

        [TestCase("Docs/mytest_edit_annotation.pdf", 62, 795, "First paragraph")]
        public void GetCharactersBounds_ShouldReturnBoundsOfText(string filePath, int x1, int y1, string text)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                //获得文字的矩形区域
                var actual = pageReader.GetCharactersBounds(0, text.Length);
                Assert.IsTrue(actual.Count > 0);
                Assert.IsTrue(actual[0].IsContainPoint(new PointF(x1, y1)));
                //选择矩形区域
                var selection0 = pageReader.GetSelection(actual[0]);
                Assert.IsTrue(selection0.Text.Contains("First"));
                var selection1 = pageReader.GetSelection(actual[1]);
                Assert.IsTrue(selection1.Text.Contains("paragraph"));
                var selections = selection0.Clone() as PdfSelection;
                selections.AddSelection(selection1);
                Assert.IsTrue(selections.Text.Contains("First paragraph"));
                var attrStrs = selections.AttributedString;
                foreach (var attr in attrStrs)
                {
                    attr.Page = pageReader;
                    var fontSize = attr.FontSize;
                    var strokeColor = attr.StrokeColor;
                    Assert.IsTrue(strokeColor.A == Color.Black.A &&
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

        [TestCase("Docs/mytest_edit_annotation.pdf", 73, 795, "p")]
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
                Pdfium.Instance.Save(pageReader.Document, "Result.pdf");
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
    }
}
