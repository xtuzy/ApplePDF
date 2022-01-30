using System;
using System.Linq;
using System.Threading.Tasks;
using Pdf.Net.PdfKit;
using NUnit.Framework;
using PDFiumCore;

namespace Pdf.Net.Test
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
        public void PageIndex_WhenCalled_ShouldReturnCorrectIndex()
        {
            var random = new Random();

            var index = random.Next(19);

            ExecuteForDocument("Docs/simple_0.pdf", null, index, pageReader =>
             {
                 Assert.AreEqual(index, pageReader.PageIndex);
             });
        }

        [Theory]
        public void GetCharacters_WhenCalled_ShouldReturnCorrectCharacters()
        {
            ExecuteForDocument("Docs/simple_6.pdf", null, 0, pageReader =>
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

                Assert.AreEqual("Horizontal", firstText);

                var secondText = string.Empty;

                for (var i = 12; i < 20; i++)
                {
                    var ch = characters[i];

                    Assert.AreEqual(12, ch.FontSize);
                    Assert.AreEqual(4.712, ch.Angle, 3);

                    secondText += ch.Char;
                }

                Assert.AreEqual("Vertical", secondText);
            });
        }

        [Theory]
        [TestCase("Docs/simple_2.pdf", 1, "2")]
        [TestCase("Docs/simple_2.pdf", 3, "4 CONTENTS")]
        [TestCase("Docs/simple_4.pdf", 0, "")]
        [TestCase("Docs/simple_5.pdf", 0, "test.md 11/11/2018\r\n1 / 1\r\nTest document")]
        [TestCase("Docs/simple_2.pdf", 1, "2")]
        [TestCase("Docs/simple_2.pdf", 3, "4 CONTENTS")]
        [TestCase("Docs/simple_4.pdf", 0, "")]
        [TestCase("Docs/simple_5.pdf", 0, "test.md 11/11/2018\r\n1 / 1\r\nTest document")]
        public void GetText_WhenCalled_ShouldReturnValidText(string filePath, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, null, pageIndex, pageReader =>
            {
                var text = pageReader.Text;

                Assert.AreEqual(expectedText, text);
            });
        }

        [Theory]
        [TestCase("Docs/simple_3.pdf", null, 1, "Simple PDF File 2")]
        [TestCase("Docs/simple_3.pdf", null, 1, "Boring. More,")]
        [TestCase("Docs/simple_3.pdf", null, 1, "The end, and just as well.")]
        [TestCase("Docs/simple_0.pdf", null, 4, "ASCIIHexDecode")]
        [TestCase("Docs/protected_0.pdf", "password", 0, "The Secret (2016 film)")]
        [TestCase("Docs/simple_3.pdf", null, 1, "Simple PDF File 2")]
        [TestCase("Docs/simple_3.pdf", null, 1, "Boring. More,")]
        [TestCase("Docs/simple_3.pdf", null, 1, "The end, and just as well.")]
        [TestCase("Docs/simple_0.pdf", null, 4, "ASCIIHexDecode")]
        [TestCase("Docs/protected_0.pdf", "password", 0, "The Secret (2016 film)")]
        public void GetText_WhenCalled_ShouldContainValidText(string filePath, string password, int pageIndex, string expectedText)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
          {
              var text = pageReader.Text;
              var c = text.Contains(expectedText, StringComparison.Ordinal);
              Assert.AreEqual(true, c);
          });
        }

        [Theory]
        [TestCase("Docs/simple_2.pdf", null, 1, 1)]
        [TestCase("Docs/simple_2.pdf", null, 3, 10)]
        [TestCase("Docs/simple_5.pdf", null, 0, 40)]
        [TestCase("Docs/protected_0.pdf", "password", 0, 2009)]
        [TestCase("Docs/simple_2.pdf", null, 1, 1)]
        [TestCase("Docs/simple_2.pdf", null, 3, 10)]
        [TestCase("Docs/simple_5.pdf", null, 0, 40)]
        [TestCase("Docs/protected_0.pdf", "password", 0, 2009)]
        public void GetCharacters_WhenCalled_ShouldReturnCharacters(string filePath, string password, int pageIndex, int charCount)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var characters = pageReader.GetCharacters().ToArray();

                Assert.AreEqual(charCount, characters.Length);
            });
        }

        [Theory]
        [TestCase("Docs/simple_3.pdf", null, 1)]
        [TestCase("Docs/simple_0.pdf", null, 18)]
        [TestCase("Docs/protected_0.pdf", "password", 0)]
        [TestCase("Docs/simple_3.pdf", null, 1)]
        [TestCase("Docs/simple_0.pdf", null, 18)]
        [TestCase("Docs/protected_0.pdf", "password", 0)]
        public void GetImage_WhenCalled_ShouldReturnNonZeroRawByteArray(string filePath, string password, int pageIndex)
        {
            ExecuteForDocument(filePath, password, pageIndex, pageReader =>
            {
                var bytes = pageReader.GetImage(1f, 1f, 0).ToArray();

                Assert.True(bytes.Length > 0);
                Assert.IsNotEmpty(bytes.Where(x => x != 0));
            });
        }

        [Theory]
        [TestCase("Docs/simple_3.pdf", null, 1)]
        [TestCase("Docs/simple_0.pdf", null, 18)]
        [TestCase("Docs/protected_0.pdf", "password", 0)]
        [TestCase("Docs/simple_3.pdf", null, 1)]
        [TestCase("Docs/simple_0.pdf", null, 18)]
        [TestCase("Docs/protected_0.pdf", "password", 0)]
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
            var task1 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/simple_0.pdf", _fixture), 2000000, 2400000));
            var task2 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/simple_1.pdf", _fixture), 190000, 200000));
            var task3 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/simple_2.pdf", _fixture), 4500, 4900));
            var task4 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/simple_3.pdf", _fixture), 20000, 22000));
            var task5 = Task.Run(() => InRange(GetNonZeroByteCount("Docs/simple_4.pdf", _fixture), 0, 0));

            Task.WaitAll(task1, task2, task3, task4, task5);
        }

        void InRange(int actual, int small, int big)
        {
            Assert.GreaterOrEqual(actual, small);
            Assert.LessOrEqual(actual, big);
        }

        [Theory]
        [TestCase("Docs/simple_0.pdf", null, 1, 595, 841)]
        [TestCase("Docs/simple_0.pdf", null, 10, 5953, 8419)]
        [TestCase("Docs/simple_0.pdf", null, 15, 8929, 12628)]
        public void GetPageWidthOrHeight_WhenCalledWithScalingFactor_ShouldMach(string filePath, string password, double scaling, int expectedWidth, int expectedHeight)
        {
            ExecuteForDocument(filePath, password, 0, (Action<PdfPage>)(pageReader =>
           {
               var rect = pageReader.GetBoundsForBox();
               var width = rect.Width * scaling;
               var height = rect.Height * scaling;

               Assert.AreEqual(expectedWidth, width, 1);
               Assert.AreEqual(expectedHeight, height, 1);
           }));
        }

        [Test]
        public void GetImage_WhenCalledWithoutRenderAnnotationsFlag_ShouldNotRenderAnnotation()
        {
            ExecuteForDocument("Docs/annotation_0.pdf", null, 0, pageReader =>
           {
               var bytes = pageReader.GetImage(1, 1, 0).ToArray();
               Assert.True(bytes.All(x => x == 0));
           });
        }

        [Test]
        public void GetImage_WhenCalledWithRenderAnnotationsFlag_ShouldRenderAnnotation()
        {
            ExecuteForDocument("Docs/annotation_0.pdf", null, 0, pageReader =>
           {
               // verify pixel in center of image is the correct yellow color
               var bytes = pageReader.GetImage(1, 1, (int)RenderFlags.RenderAnnotations).ToArray();
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
            ExecuteForDocument("Docs/annotation_0.pdf", null, 0, pageReader =>
           {
               // verify pixel in center of image is the correct gray color
               var bytes = pageReader.GetImage(1, 1, (int)(RenderFlags.RenderAnnotations | RenderFlags.Grayscale)).ToArray();
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
                    var rect = pageReader.GetBoundsForBox();
                    var width = rect.Width;
                    var height = rect.Height;

                    var scaleOne = DimOne / Math.Min(width, height);
                    var scalingTwo = DimTwo / Math.Max(width, height);

                    var scale = Math.Min(scaleOne, scalingTwo);
                    //return pageReader.GetImage().Count(x => x != 0);
                    return pageReader.GetImage(scale, scale, 0).Count(x => x != 0);
                }
            }
        }
     
        [TestCase("Docs/mytest_4_highlightannotation.pdf", 4)]
        [TestCase("Docs/mytest_5_inkannotation.pdf", 5)]
        [TestCase("Docs/mytest_4_freetextannotation.pdf", 4)]
        [TestCase("Docs/mytest_4_rectangleannotation.pdf", 4)]
        [TestCase("Docs/mytest_4_linkannotation.pdf", 4)]
        public void Annotations_WhenCalled_ShouldGetCurrectAnnotationsCount(string filePath,int annotationsCount)
        {
            ExecuteForDocument(filePath, null, 0, pageReader =>
            {
                var annots = pageReader.Annotations;
                Assert.AreEqual(annotationsCount, annots.Count);
            });
        }
    }
}
