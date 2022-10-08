
namespace ApplePDF.PdfKit
{
    using ApplePDF.PdfKit.Annotation;
    using PDFiumCore;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    /// <summary>
    /// 注意：
    /// Pdf的坐标系原点是左下角，此库中获取得到的<see cref="Point"/>都是基于此，返回<see cref="Rectangle"/>的由于其来自System.Drawing，其基于左上角原点，请只使用其上下左右的位置信息，不要使用大小信息，否则会出现大小为负数。
    /// </summary>
    public class PdfPage : IPdfPage_Pdfium, IDisposable
    {
        private static readonly object @lock = new object();
        public PdfDocument Document { get; private set; }
        /// <summary>
        /// Pdfium中代表页面,获取页面中的东西时需要它
        /// </summary>
        private FpdfPageT page;
        /// <summary>
        /// Pdfium中代表页面的文本,获取文本时需要它
        /// </summary>
        private FpdfTextpageT textPage;
        public FpdfTextpageT TextPage
        {
            get
            {
                if (textPage == null)
                    textPage = fpdf_text.FPDFTextLoadPage(this.page);
                return textPage;
            }
            private set => textPage = value;
        }

        private SizeF size;
        public int PageIndex { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPage"/> class.
        /// </summary>
        /// <param name="doc">doc instance</param>
        /// <param name="index">page index in doc</param>
        public PdfPage(PdfDocument doc, int index)
        {
            this.Document = doc;
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
                    return fpdf_text.FPDFTextCountChars(this.TextPage);
                }
            }
        }

        public bool DisplaysAnnotations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FpdfPageT Page => page;

        public PdfRotate Rotation { get => (PdfRotate)fpdf_edit.FPDFPageGetRotation(Page); set => fpdf_edit.FPDFPageSetRotation(Page, (int)value); }

        #region 注释

        public int AnnotationCount
        {
            get
            {
                return fpdf_annot.FPDFPageGetAnnotCount(Page);
            }
        }

        public List<PdfAnnotation> Annotations
        {
            get
            {
                var count = fpdf_annot.FPDFPageGetAnnotCount(Page);
                List<PdfAnnotation> annotations = new List<PdfAnnotation>();

                for (var index = 0; index < count; index++)
                {
                    var annotation = fpdf_annot.FPDFPageGetAnnot(this.Page, index);
                    var annotationType = (PdfAnnotationSubtype)fpdf_annot.FPDFAnnotGetSubtype(annotation);
                    switch (annotationType)
                    {
                        case PdfAnnotationSubtype.Text:
                            break;
                        case PdfAnnotationSubtype.Link:
                            annotations.Add(new PdfLinkAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.FreeText:
                            annotations.Add(new PdfFreeTextAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Line:
                            annotations.Add(new PdfLineAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Square:
                            annotations.Add(new PdfSquareAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Circle:
                            annotations.Add(new PdfCircleAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Highlight:
                            annotations.Add(new PdfHighlightAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Underline:
                            annotations.Add(new PdfUnderlineAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.StrikeOut:
                            break;
                        case PdfAnnotationSubtype.Ink:
                            annotations.Add(new PdfInkAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Stamp:
                            annotations.Add(new PdfStampAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Popup://Popup附着在其它注释上
                            //annotations.Add(new PdfPopupAnnotation(this, annotation, annotationType, index));
                            break;
                        case PdfAnnotationSubtype.Widget:
                            annotations.Add(new PdfWidgetAnnotation(this, annotation, annotationType, index));
                            break;
                    }
                }
                return annotations;
            }
        }

        public void AddAnnotation(PdfAnnotation annotation)
        {
            annotation.AddToPage(this);
            Annotations.Add(annotation);
        }

        public PdfAnnotation GetAnnotations(PointF point)
        {
            throw new NotImplementedException();
        }

        public void RemoveAnnotation(PdfAnnotation annotation)
        {
            var index = fpdf_annot.FPDFPageGetAnnotIndex(Page, annotation.Annotation);
            fpdf_annot.FPDFPageRemoveAnnot(Page, index);
        }

        #endregion

        #region Set文本

        public bool InsteadText(string oldText, string newText)
        {
            var objectCount = fpdf_edit.FPDFPageCountObjects(Page);

            ushort[] buffer = new ushort[1];
            for (var index = 0; index < objectCount; index++)
            {
                var objectInPage = fpdf_edit.FPDFPageGetObject(Page, index);
                var type = fpdf_edit.FPDFPageObjGetType(objectInPage);
                if (type == (int)PdfPageObjectTypeFlag.TEXT)
                {
                    //获取文本长度
                    var length = fpdf_edit.FPDFTextObjGetText(objectInPage, TextPage, ref buffer[0], 1);
                    if (length > 0)
                    {
                        buffer = new ushort[length];
                        fpdf_edit.FPDFTextObjGetText(objectInPage, TextPage, ref buffer[0], length);
                        //参考:https://stackoverflow.com/a/274207/13254773
                        string result;
                        unsafe
                        {
                            fixed (ushort* dataPtr = &buffer[0])
                            {
                                result = new string((char*)dataPtr, 0, (int)length - 1);
                            }
                        }

                        if (result.Contains(oldText))
                        {
                            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
                            //var newTextBytes = Encoding.Unicode.GetBytes(newText);
                            //ushort[] newTextBuffer = new ushort[newText.Length];
                            //Buffer.BlockCopy(newTextBytes, 0, newTextBuffer, 0, newTextBytes.Length);

                            //string to ushort 参考:https://stackoverflow.com/a/45281549/13254773
                            ushort[] newTextBuffer = (newText + result.Substring(oldText.Length)).ToCharArray().Select(c => (ushort)c).ToArray();

                            var success = fpdf_edit.FPDFTextSetText(objectInPage, ref newTextBuffer[0]);
                            if (success == 1)
                                return true;
                            break;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 使用字体插入文本(不是注释,是作为正文)
        /// </summary>
        /// <param name="font">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="text">文本</param>
        /// <param name="x">左下角为原点</param>
        /// <param name="y">左下角为原点</param>
        /// <param name="scale">缩放</param>
        /// <returns>是否成功</returns>
        public bool AddText(PdfFont font, float fontSize, string text, double x, double y, double scale = 1)
        {
            var textObj = fpdf_edit.FPDFPageObjCreateTextObj(Document.Document, font.Font, fontSize);
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var newTextBytes = Encoding.Unicode.GetBytes(text);
            ushort[] newTextBuffer = new ushort[text.Length];
            Buffer.BlockCopy(newTextBytes, 0, newTextBuffer, 0, newTextBytes.Length);

            //string to ushort 参考:https://stackoverflow.com/a/45281549/13254773
            //ushort[] newTextBuffer = text.ToCharArray().Select(c => (ushort)c).ToArray();

            var success = fpdf_edit.FPDFTextSetText(textObj, ref newTextBuffer[0]);
            if (success == 1)
            {
                //设置位置
                fpdf_edit.FPDFPageObjTransform(textObj, scale, 0, 0, scale, 200, 200);
                fpdf_edit.FPDFPageInsertObject(Page, textObj);
                return true;
            }
            return false;
        }

        #endregion Set文本

        #region Get文本 Text

        public string Text
        {
            get
            {
                lock (@lock)
                {
                    return this.GetTextInPage(0, this.CharacterCount);
                }
            }
        }

        /// <summary>
        /// 获得Bounds,即获得宽高,其单位为dp
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public SizeF GetSize()
        {
            lock (@lock)
            {
                if (size == default)
                {
                    var w = fpdfview.FPDF_GetPageWidthF(this.page);
                    var h = fpdfview.FPDF_GetPageHeightF(this.page);
                    size = new SizeF(w, h);
                }
                return size;
            }
        }

        public RectangleF GetBoundsForBox(PdfDisplayBox pdfDisplayBox)
        {
            float left = 0;
            float top = 0;
            float right = 0;
            float bottom = 0;
            switch (pdfDisplayBox)
            {
                case PdfDisplayBox.Media:
                    fpdf_transformpage.FPDFPageGetMediaBox(Page, ref left, ref bottom, ref right, ref top);
                    break;
                case PdfDisplayBox.Crop:
                    fpdf_transformpage.FPDFPageGetCropBox(Page, ref left, ref bottom, ref right, ref top);
                    break;
                case PdfDisplayBox.Bleed:
                    fpdf_transformpage.FPDFPageGetBleedBox(Page, ref left, ref bottom, ref right, ref top);
                    break;
                case PdfDisplayBox.Trim:
                    fpdf_transformpage.FPDFPageGetTrimBox(Page, ref left, ref bottom, ref right, ref top);
                    break;
                case PdfDisplayBox.Art:
                    fpdf_transformpage.FPDFPageGetArtBox(Page, ref left, ref bottom, ref right, ref top);
                    break;
            }
            return RectangleF.FromLTRB(left, top, right, bottom);
        }

        //In "User Sapce"
        public RectangleF GetCharacterBounds(int index)
        {
            double left = 0;
            double right = 0;
            double bottom = 0;
            double top = 0;
            var result = fpdf_text.FPDFTextGetCharBox(TextPage, index, ref left, ref right, ref bottom, ref top);
            if (result == 0)
                return RectangleF.Empty;
            return RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom);
        }

        /// <summary>
        /// 获取连续字符的边框。在需要快速确定区域时(比如选择文字高亮时)可能用到。
        /// <see cref="fpdf_text.FPDFTextCountRects">:This function, along with FPDFText_GetRect can be used by applications to detect the position on the page for a text segment, so proper areas can be highlighted. The FPDFText_* functions will automatically merge small character boxes into bigger one if those characters are on the same line and use same font settings.
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<RectangleF> GetCharactersBounds(int index, int count)
        {
            var result = new List<RectangleF>();
            if (index < CharacterCount && (index + count) < CharacterCount)
            {
                int rectCount = fpdf_text.FPDFTextCountRects(TextPage, index, count);
                if (rectCount > 0)
                {
                    for (int rectIndex = 0; rectIndex < rectCount; rectIndex++)
                    {
                        double x1 = 0.0; double y1 = 0.0; double x2 = 0.0; double y2 = 0.0;

                        if(fpdf_text.FPDFTextGetRect(TextPage, rectIndex, ref x1, ref y1, ref x2, ref y2) == 0 )
                            throw new NotImplementedException("Get a bounds info not success, this let the count of bounds not equal to count of rects when GetCharactersBounds");

                        RectangleF rect = RectangleF.FromLTRB((float)x1, (float)y1, (float)x2, (float)y2);

                        result.Add(rect);
                    }
                }
            }
            return result;
        }

        public int GetCharacterIndex(PointF point)
        {
            var result = fpdf_text.FPDFTextGetCharIndexAtPos(this.TextPage, point.X, point.Y, 5, 5);//TODO:寻找最佳x，y tolerance
            if (result == -1)
                return -1;
            else if (result == -3)
                throw new NotImplementedException("Error occur when GetCharacterIndex, not show reason");
            else
                return result;
        }

        public PdfSelection GetSelection(PointF leftPoint, PointF rightPoint)
        {
            if(leftPoint.Y < rightPoint.Y)
                return new PdfSelection(this, RectangleF.FromLTRB(leftPoint.X, rightPoint.Y, rightPoint.X, leftPoint.Y));
            return new PdfSelection(this, RectangleF.FromLTRB(leftPoint.X, leftPoint.Y, rightPoint.X, rightPoint.Y));
        }

        public PdfSelection GetSelection(RectangleF rect)
        {
            return new PdfSelection(this, rect);
        }

        public PdfSelection SelectLine(PointF point)
        {
            //获得该位置的字符序号
            var index = GetCharacterIndex(point);
            //字符大小作为行高度
            //var textSize = fpdf_text.FPDFTextGetFontSize(TextPage, index);
            var charBounds = GetCharacterBounds(index);
            var size = this.GetSize();
            return new PdfSelection(this, new RectangleF(0, charBounds.Y, size.Width, charBounds.Height));//TODO:验证取行宽和字符高度确定行的逻辑是否正确
        }

        //TODO:当前只获得到char，需要加入分词
        public PdfSelection SelectWord(PointF point)
        {
            //获得该位置的字符序号
            var index = GetCharacterIndex(point);
            return new PdfSelection(this, GetCharacterBounds(index));
        }

        private string GetTextInPage(int fromeIndex, int count = 1)
        {
            lock (@lock)
            {
                ushort[] buffer;
                int charactersWritten;
                buffer = new ushort[count + 1];//+1是为了存一个结束字符？
                charactersWritten = fpdf_text.FPDFTextGetText(this.TextPage, fromeIndex, count, ref buffer[0]);

                if (charactersWritten == 0)
                {
                    return string.Empty;
                }

                //参考:https://stackoverflow.com/a/274207/13254773
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
                var bounds = GetSize();
                var width = (int)bounds.Width;
                var height = (int)bounds.Height;

                var charCount = CharacterCount;

                for (var i = 0; i < charCount; i++)
                {
                    var charCode = (char)fpdf_text.FPDFTextGetUnicode(TextPage, i);

                    double left = 0;
                    double top = 0;
                    double right = 0;
                    double bottom = 0;

                    var success = fpdf_text.FPDFTextGetCharBox(TextPage, i, ref left, ref right, ref bottom, ref top) == 1;

                    if (!success)
                    {
                        continue;
                    }

                    var (adjustedLeft, adjustedTop) = GetAdjustedCoords(width, height, left, top);
                    var (adjustRight, adjustBottom) = GetAdjustedCoords(width, height, right, bottom);

                    var box = new RectangleF(adjustedLeft, adjustedTop, adjustRight - adjustedLeft, adjustBottom - adjustedTop);

                    var fontSize = fpdf_text.FPDFTextGetFontSize(TextPage, i);
                    var angle = fpdf_text.FPDFTextGetCharAngle(TextPage, i);

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
        /// 获取页面图像。图像的内存由Pdfium开辟
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="renderFlag">RenderFlag, can use &| combine; if set 0, not render annotation</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] Draw(float xScale, float yScale, int renderFlag)
        {
            lock (@lock)
            {
                // Get Metrics
                var bounds = GetSize();
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

        /// <summary>
        /// 应用开辟内存给Pdfium存储图像
        /// </summary>
        /// <param name="imageBufferPointer"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="rotate"></param>
        /// <param name="renderFlag"><see cref="RenderFlags"/>,可以叠加，如`RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting`</param>
        /// <exception cref="Exception"></exception>
        public void Draw(IntPtr imageBufferPointer, float xScale, float yScale, int rotate, int renderFlag)
        {
            lock (@lock)
            {
                // Get Metrics
                var bounds = GetSize();
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

                        fpdfview.FPDF_RenderPageBitmapWithMatrix(bitmap, Page, matrix, clipping, renderFlag);
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
        /// <summary>
        /// 保存对页面内容的操作,如增加文本
        /// </summary>
        /// <returns></returns>
        public bool SaveNewContent()
        {
            return fpdf_edit.FPDFPageGenerateContent(Page) == 1 ? true : false;
        }

        public void Dispose()
        {
            lock (@lock)
            {
                if (TextPage != null)
                {
                    fpdf_text.FPDFTextClosePage(TextPage);
                    TextPage = null;
                }
                if (page != null)
                {
                    fpdfview.FPDF_ClosePage(page);
                    page = null;
                }
                Document = null;
            }
        }

        public object GetThumbnail(Size size, PdfDisplayBox box)
        {
            var bitmap = fpdf_thumbnail.FPDFPageGetThumbnailAsBitmap(this.page);
            int height = 0;
            int width = 0;
            if (bitmap != null)
            {
                var buffer = fpdfview.FPDFBitmapGetBuffer(bitmap);
                height = fpdfview.FPDFBitmapGetHeight(bitmap);
                width = fpdfview.FPDFBitmapGetWidth(bitmap);
                var stride = fpdfview.FPDFBitmapGetStride(bitmap);
                var result = new byte[stride * height];
                try
                {
                    Marshal.Copy(buffer, result, 0, result.Length);
                }
                catch (Exception) { result = new byte[1]; }
                finally
                {
                    fpdfview.FPDFBitmapDestroy(bitmap);
                }
                if (result.Length > 1)
                {
                    return result;//TODO:缩放到Size
                }
            }
            //没有生成Pdf自带的缩略图时,我们自己生成页面图像
            var pageSize = GetSize();
            return Draw(size.Width / pageSize.Width, size.Height / pageSize.Height, (int)RenderFlags.None);
        }
    }
}