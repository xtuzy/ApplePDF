
namespace Pdf.Net.PdfKit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using PDFiumCore;

    public class PdfPage : IPdfPage, IDisposable
    {
        private static readonly object @lock = new object();
        private PdfDocument doc;
        private FpdfPageT page;
        private FpdfTextpageT textPage;
        private RectangleF bounds;
        public int PageIndex { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPage"/> class.
        /// </summary>
        /// <param name="doc">doc instance</param>
        /// <param name="index">page index in doc</param>
        public PdfPage(PdfDocument doc, int index)
        {
            this.doc = doc;
            this.PageIndex = index;
            lock (@lock)
            {
                this.page = fpdfview.FPDF_LoadPage(doc.Document, index);
            }
        }

        public int CharacterCount
        {
            get
            {
                lock (@lock)
                {
                    if (this.textPage == null)
                    {
                        this.textPage = fpdf_text.FPDFTextLoadPage(this.page);
                    }

                    return fpdf_text.FPDFTextCountChars(this.textPage);
                }
            }
        }

        public bool DisplaysAnnotations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FpdfPageT Page => page;

        public PdfRotate Rotation { get => (PdfRotate)fpdf_edit.FPDFPageGetRotation(Page); set => fpdf_edit.FPDFPageSetRotation(Page,(int)value); }

        #region 注释

        public List<PdfAnnotation> Annotations
        {
            get
            {
                var count = fpdf_annot.FPDFPageGetAnnotCount(Page);
                List<PdfAnnotation> annotations = new List<PdfAnnotation>();
                for (var index = 0; index < count; index++)
                {
                    annotations.Add(new PdfAnnotation(this, index));
                }
                return annotations;
            }
        }

        public void AddAnnotation(PdfAnnotation annotation)
        {
            if (annotation.Page != this)
                throw new ArgumentException("Add Annotation to uncorrect page.");
            var anno = fpdf_annot.FPDFPageCreateAnnot(this.Page, (int)annotation.AnnotationType);
            annotation.Annotation = anno;
            var index = fpdf_annot.FPDFPageGetAnnotIndex(Page, annotation.Annotation);
            annotation.Index = index;
            Annotations.Add(annotation);
        }

        public PdfAnnotation GetAnnotations(Point point)
        {
            throw new NotImplementedException();

        }

        public void RemoveAnnotation(PdfAnnotation annotation)
        {
            var index = fpdf_annot.FPDFPageGetAnnotIndex(Page, annotation.Annotation);
            fpdf_annot.FPDFPageRemoveAnnot(Page, index);
        }

        #endregion

        #region 文本 Text

        public string Text
        {
            get
            {
                lock (@lock)
                {
                    if (this.textPage != null)
                    {
                        this.textPage = fpdf_text.FPDFTextLoadPage(this.page);
                    }
                    return this.GetText();
                }
            }
        }

        /// <summary>
        /// 获得Bounds,即获得宽高,其单位为dp
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public RectangleF GetBoundsForBox(PdfDisplayBox box = PdfDisplayBox.Media)
        {
            lock (@lock)
            {
                if (bounds == default)
                {
                    var w = fpdfview.FPDF_GetPageWidthF(this.page);
                    var h = fpdfview.FPDF_GetPageHeightF(this.page);
                    bounds = new RectangleF(0, 0, w, h);
                }
                return bounds;
            }
        }

        public RectangleF GetCharacterBounds(int index)
        {
            throw new NotImplementedException();
        }

        public int GetCharacterIndex(Point point)
        {
            throw new NotImplementedException();
        }

        public PdfSelection GetSelection(Point startPoint, Point endPoint)
        {
            throw new NotImplementedException();
        }

        public PdfSelection GetSelection(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public PdfSelection SelectLine(Point point)
        {
            throw new NotImplementedException();
        }

        public PdfSelection SelectWord(Point point)
        {
            throw new NotImplementedException();
        }

        private string GetText()
        {
            lock (@lock)
            {
                ushort[] buffer;
                int charactersWritten;
                buffer = new ushort[this.CharacterCount + 1];
                charactersWritten = fpdf_text.FPDFTextGetText(this.textPage, 0, this.CharacterCount, ref buffer[0]);

                if (charactersWritten == 0)
                {
                    return string.Empty;
                }

                string result;
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                    {
                        result = new string((char*)dataPtr, 0, charactersWritten - 1);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 获取页面中每个字符的精确数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PdfCharacter> GetCharacters()
        {
            lock (@lock)
            {
                var bounds = GetBoundsForBox();
                var width = (int)bounds.Width;
                var height = (int)bounds.Height;

                var charCount = CharacterCount;

                for (var i = 0; i < charCount; i++)
                {
                    var charCode = (char)fpdf_text.FPDFTextGetUnicode(textPage, i);

                    double left = 0;
                    double top = 0;
                    double right = 0;
                    double bottom = 0;

                    var success = fpdf_text.FPDFTextGetCharBox(textPage, i, ref left, ref right, ref bottom, ref top) == 1;

                    if (!success)
                    {
                        continue;
                    }

                    var (adjustedLeft, adjustedTop) = GetAdjustedCoords(width, height, left, top);
                    var (adjustRight, adjustBottom) = GetAdjustedCoords(width, height, right, bottom);

                    var box = new RectangleF(adjustedLeft, adjustedTop, adjustRight - adjustedLeft, adjustBottom - adjustedTop);

                    var fontSize = fpdf_text.FPDFTextGetFontSize(textPage, i);
                    var angle = fpdf_text.FPDFTextGetCharAngle(textPage, i);

                    yield return new PdfCharacter(charCode, box, angle, fontSize);
                }
            }
        }

        private (int x, int y) GetAdjustedCoords(int width, int height, double pageX, double pageY)
        {
            var x = 0;
            var y = 0;

            fpdfview.FPDF_PageToDevice(
                page, 0, 0, width, height, 0, pageX, pageY, ref x, ref y);
            x = AdjustToRange(x, width);
            y = AdjustToRange(y, height);
            return (x, y);
        }

        /// <summary>
        /// TODO:弄懂这旧代码是做啥的
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private int AdjustToRange(int coord, int range)
        {
            if (coord < 0)
            {
                coord = 0;
            }

            if (coord >= range)
            {
                coord = range - 1;
            }

            return coord;
        }

        #endregion 文本 Text

        #region 渲染 Render

        /// <summary>
        /// 获取页面图像
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="renderFlag">RenderFlag, can use &| combine; if set 0, not render annotation</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] GetImage(float xScale, float yScale, int renderFlag)
        {
            lock (@lock)
            {
                // Get Metrics
                var bounds = GetBoundsForBox();
                int width = (int)(bounds.Width * xScale);
                int height = (int)(bounds.Height * yScale);
                var bitmap = fpdfview.FPDFBitmapCreate(width, height, 1);
                if (bitmap == null)
                {
                    throw new Exception("failed to create a bitmap object");
                }

                var stride = fpdfview.FPDFBitmapGetStride(bitmap);

                var result = new byte[stride * height];

                try
                {
                    // |          | a b 0 |
                    // | matrix = | c d 0 |
                    // |          | e f 1 |
                    using (var matrix = new FS_MATRIX_())
                    using (var clipping = new FS_RECTF_())
                    {
                        // 使用矩阵对页面进行缩放,使页面适应图片大小
                        matrix.A = xScale;
                        matrix.B = 0;
                        matrix.C = 0;

                        matrix.D = yScale;
                        matrix.E = 0;
                        matrix.F = 0;

                        clipping.Left = 0;
                        clipping.Right = width;
                        clipping.Bottom = 0;
                        clipping.Top = height;

                        fpdfview.FPDF_RenderPageBitmapWithMatrix(bitmap, page, matrix, clipping, renderFlag);

                        var buffer = fpdfview.FPDFBitmapGetBuffer(bitmap);

                        Marshal.Copy(buffer, result, 0, result.Length);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("error rendering page", ex);
                }
                finally
                {
                    fpdfview.FPDFBitmapDestroy(bitmap);
                }

                return result;
            }
        }

        public void GetImage(IntPtr imageBufferPointer, float xScale, float yScale, int rotate, int renderFlag)
        {
            lock (@lock)
            {
                // Get Metrics
                var bounds = GetBoundsForBox();
                int width = (int)(bounds.Width * xScale);
                int height = (int)(bounds.Height * yScale);
                var bitmap = fpdfview.FPDFBitmapCreateEx(width, height, (int)FPDFBitmapFormat.BGRA, imageBufferPointer, width * 4);
                if (bitmap == null)
                {
                    throw new Exception("failed to create a bitmap object");
                }

                try
                {
                    // |          | a b 0 |
                    // | matrix = | c d 0 |
                    // |          | e f 1 |
                    using (var matrix = new FS_MATRIX_())
                    using (var clipping = new FS_RECTF_())
                    {
                        // 使用矩阵对页面进行缩放,使页面适应图片大小
                        matrix.A = xScale;
                        matrix.B = 0;
                        matrix.C = 0;

                        matrix.D = yScale;
                        matrix.E = 0;
                        matrix.F = 0;

                        clipping.Left = 0;
                        clipping.Right = width;
                        clipping.Bottom = 0;
                        clipping.Top = height;

                        fpdfview.FPDF_RenderPageBitmapWithMatrix(bitmap, Page, matrix, clipping, (int)RenderFlags.RenderAnnotations);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("error rendering page", ex);
                }
                finally
                {
                    fpdfview.FPDFBitmapDestroy(bitmap);
                }
            }
        }

        #endregion

        public void Dispose()
        {
            lock (@lock)
            {
                if (textPage != null)
                {
                    fpdf_text.FPDFTextClosePage(textPage);
                    textPage = null;
                }
                if (page != null)
                {
                    fpdfview.FPDF_ClosePage(page);
                    page = null;
                }
                doc = null;
            }
        }

    }
}