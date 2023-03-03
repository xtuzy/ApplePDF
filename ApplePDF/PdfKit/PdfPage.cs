
namespace ApplePDF.PdfKit
{
    using ApplePDF.PdfKit.Annotation;
    using Microsoft.Maui.Graphics.Text;
    using PDFiumCore;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Runtime.InteropServices;
    using System.Text;
    /// <summary>
    /// 注意：
    /// Pdf的坐标系原点是左下角，此库中获取得到的<see cref="Point"/>都是基于此，返回<see cref="Rectangle"/>的由于其来自System.Drawing，其基于左上角原点，请只使用其上下左右的位置信息，不要使用大小信息，否则会出现大小为负数。
    /// </summary>
    public class PdfPage : IPdfPage
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
        internal PdfPage(PdfDocument doc, int index)
        {
            this.Document = doc;
            this.PageIndex = index;
            lock (@lock)
            {
                this.page = fpdfview.FPDF_LoadPage(doc.Document, index);
            }
        }

        internal PdfPage(PdfDocument doc, int index, FpdfPageT platformPage)
        {
            this.Document = doc;
            this.PageIndex = index;
            page = platformPage;
        }

        public bool DisplaysAnnotations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FpdfPageT Page
        {
            get
            {
                if (page == null)
                    throw new ObjectDisposedException(nameof(Page));
                return page;
            }
        }

        public PdfRotate Rotation { get => (PdfRotate)fpdf_edit.FPDFPageGetRotation(Page); set => fpdf_edit.FPDFPageSetRotation(Page, (int)value); }

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

        public PdfRectangleF GetBoundsForBox(PdfDisplayBox pdfDisplayBox)
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
            return PdfRectangleF.FromLTRB(left, top, right, bottom);
        }

        #region 注释

        public int AnnotationCount => fpdf_annot.FPDFPageGetAnnotCount(Page);

        public PdfAnnotation[] GetAnnotations()
        {
            var count = AnnotationCount;
            if (count == 0)
                return null;
            var annotations = new List<PdfAnnotation>() { Capacity = count };
            for (var index = 0; index < count; index++)
            {
                var annot = GetAnnotation(index);
                if (annot != null)
                    annotations.Add(annot);
            }
            return annotations.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">form 0 to <see cref="AnnotationCount"></param>
        /// <returns></returns>
        public PdfAnnotation GetAnnotation(int index)
        {
            var annotation = fpdf_annot.FPDFPageGetAnnot(this.Page, index);
            var annotationType = (PdfAnnotationSubtype)fpdf_annot.FPDFAnnotGetSubtype(annotation);
            switch (annotationType)
            {
                case PdfAnnotationSubtype.Text:
                    return new PdfTextAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Link:
                    return new PdfLinkAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.FreeText:
                    return new PdfFreeTextAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Line:
                    return new PdfLineAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Square:
                    return new PdfSquareAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Circle:
                    return new PdfCircleAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Highlight:
                    return new PdfHighlightAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Underline:
                    return new PdfUnderlineAnnotation(this, annotation, annotationType, index);
                    break; 
                case PdfAnnotationSubtype.Squiggly:
                    return new PdfSquigglyAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.StrikeOut:
                    return new PdfStrikeoutAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Ink:
                    return new PdfInkAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Stamp:
                    return new PdfStampAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Popup://Popup附着在其它注释上
                    break;
                case PdfAnnotationSubtype.Widget:
                    return new PdfWidgetAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Unknow:
                    return new PdfUnknowAnnotation(this, annotation, annotationType, index);
                    break;
            }
            return null;
        }

        /// <summary>
        /// Supported subtypes see <see cref="fpdf_annot.FPDFAnnotIsSupportedSubtype"/>.
        /// </summary>
        /// <param name="annotationType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public PdfAnnotation AddAnnotation(PdfAnnotationSubtype annotationType)
        {
            if (fpdf_annot.FPDFAnnotIsSupportedSubtype((int)annotationType) == 0)
                throw new NotSupportedException($"当前不支持创建{annotationType}类型的注释.");
            // 创建注释到Pdfium
            var annotation = fpdf_annot.FPDFPageCreateAnnot(Page, (int)annotationType);
            if (annotation == null)
            {
                Debug.WriteLine($"Cant't create new {annotationType} annotation");
                return null;
            }
            var index = fpdf_annot.FPDFPageGetAnnotIndex(Page, annotation);
            if (index == -1)
            {
                Debug.WriteLine($"Cant't create new {annotationType} annotation");
                return null;
            }
            switch (annotationType)
            {
                case PdfAnnotationSubtype.Text:
                    return new PdfTextAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Link:
                    return new PdfLinkAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.FreeText:
                    return new PdfFreeTextAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Line:
                    return new PdfLineAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Square:
                    return new PdfSquareAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Circle:
                    return new PdfCircleAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Highlight:
                    return new PdfHighlightAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Underline:
                    return new PdfUnderlineAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Squiggly:
                    return new PdfSquigglyAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.StrikeOut:
                    return new PdfStrikeoutAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Ink:
                    return new PdfInkAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Stamp:
                    return new PdfStampAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Popup://Popup附着在其它注释上
                    throw new InvalidOperationException("Popup associated with other type annotation, it should be created form other annotation.");
                    break;
                case PdfAnnotationSubtype.Widget:
                    return new PdfWidgetAnnotation(this, annotation, annotationType, index);
                    break;
                case PdfAnnotationSubtype.Unknow:
                    return new PdfUnknowAnnotation(this, annotation, annotationType, index);
                    break;
            }
            return null;
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

        public bool AddText(PdfFont font, float fontSize, IAttributedText text)
        {
            throw new NotImplementedException();
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
        public bool AddText(PdfFont font, float fontSize, Color color, string text, double x, double y, double scale = 1)
        {
            var textObj = PdfPageTextObj.Create(Document, font, fontSize);
           
            var success = textObj.SetText(text);
            if (success)
            {
                textObj.FillColor = color;
                textObj.SetTranform(scale, 0, 0, scale, x, y);//设置位置
                AppendObj(textObj);
                return true;
            }
            textObj.Dispose();
            return false;
        }

        #endregion Set文本

        #region Get文本

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

        //In "User Sapce"
        public PdfRectangleF GetCharacterBounds(int index)
        {
            double left = 0;
            double right = 0;
            double bottom = 0;
            double top = 0;
            var result = fpdf_text.FPDFTextGetCharBox(TextPage, index, ref left, ref right, ref bottom, ref top);
            if (result == 0)
                return PdfRectangleF.Empty;
            return PdfRectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom);
        }

        /// <summary>
        /// 获取连续字符的边框。在需要快速确定区域时(比如选择文字高亮时)可能用到。
        /// <see cref="fpdf_text.FPDFTextCountRects">:This function, along with FPDFText_GetRect can be used by applications to detect the position on the page for a text segment, so proper areas can be highlighted. The FPDFText_* functions will automatically merge small character boxes into bigger one if those characters are on the same line and use same font settings.
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public PdfRectangleF[] GetCharactersBounds(int index, int count)
        {
            
            if (index < CharacterCount && (index + count) < CharacterCount)
            {
                int rectCount = fpdf_text.FPDFTextCountRects(TextPage, index, count);
                if (rectCount > 0)
                {
                    var result = new PdfRectangleF[rectCount];
                    for (int rectIndex = 0; rectIndex < rectCount; rectIndex++)
                    {
                        double x1 = 0.0; double y1 = 0.0; double x2 = 0.0; double y2 = 0.0;

                        if (fpdf_text.FPDFTextGetRect(TextPage, rectIndex, ref x1, ref y1, ref x2, ref y2) == 0)
                            throw new NotImplementedException("Get a bounds info not success, this let the count of bounds not equal to count of rects when GetCharactersBounds");

                        var rect = PdfRectangleF.FromLTRB((float)x1, (float)y1, (float)x2, (float)y2);

                        result[rectIndex] = rect;
                    }
                    return result;
                }
            }
            return null;
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
            if (leftPoint.Y < rightPoint.Y)
                return new PdfSelection(this, PdfRectangleF.FromLTRB(leftPoint.X, rightPoint.Y, rightPoint.X, leftPoint.Y));
            return new PdfSelection(this, PdfRectangleF.FromLTRB(leftPoint.X, leftPoint.Y, rightPoint.X, rightPoint.Y));
        }

        public PdfSelection GetSelection(PdfRectangleF rect)
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
            return new PdfSelection(this, PdfRectangleF.FromLTRB(0, charBounds.Top, size.Width, charBounds.Top - charBounds.Bottom));//TODO:验证取行宽和字符高度确定行的逻辑是否正确
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

        #region 渲染

        /// <summary>
        /// 获取页面图像。原始图像的内存由Pdfium开辟, 返回的数组是从其拷贝的, 因此需要开辟两遍内存空间填充数据, 有内存拷贝损耗.
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
                var bitmap = new PdfBitmap(width, height, true);
                if (bitmap.Bitmap == null)
                {
                    throw new Exception("failed to create a bitmap object");
                }

                var result = new byte[bitmap.BufferLength];

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

                        fpdfview.FPDF_RenderPageBitmapWithMatrix(bitmap.Bitmap, page, matrix, clipping, renderFlag);

                        var buffer = bitmap.Buffer;

                        Marshal.Copy(buffer, result, 0, result.Length);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("error rendering page", ex);
                }
                finally
                {
                    bitmap.Dispose();
                }

                return result;
            }
        }

        /// <summary>
        /// 相比于<see cref="Draw(float, float, int)"/>, 这个方法内部自己创建byte[], 然后Pdfium直接在其中绘制, 因此更高效.
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="rotate"></param>
        /// <param name="renderFlag"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] Draw(float xScale, float yScale, PdfRotate rotate, int renderFlag)
        {
            // Get Metrics
            var bounds = GetSize();
            int width = (int)(bounds.Width * xScale);
            int height = (int)(bounds.Height * yScale);
            var imageBuffer = new byte[width * height * 4];
            FpdfBitmapT bitmap = null;
            unsafe
            {
                fixed (byte* p = imageBuffer)
                {
                    IntPtr ptr = (IntPtr)p;
                    // do you stuff here
                    bitmap = fpdfview.FPDFBitmapCreateEx(width, height, (int)FPDFBitmapFormat.BGRA, ptr, width * 4);//https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.writeablebitmap.pixelbuffer?view=windows-app-sdk-1.2#microsoft-ui-xaml-media-imaging-writeablebitmap-pixelbuffer 显示可以使用BGRA

                    if (bitmap == null)
                    {
                        throw new Exception("failed to create a bitmap object");
                    }

                    try
                    {
                        fpdfview.FPDF_RenderPageBitmap(bitmap, Page, 0, 0, width, height, (int)rotate, renderFlag);
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
            return imageBuffer;
        }

        /// <summary>
        /// 该方法使用Pdfium为图像分配内存, 可以使用<see cref="PdfBitmap.Buffer">访问图像数据, 没有内存拷贝的损耗; 注意使用结束需要调用<see cref="PdfBitmap.Dispose"/>释放内存.
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="renderFlag"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public PdfBitmap DrawToPdfBitmap(float xScale, float yScale, int renderFlag)
        {
            lock (@lock)
            {
                // Get Metrics
                var bounds = GetSize();
                int width = (int)(bounds.Width * xScale);
                int height = (int)(bounds.Height * yScale);
                PdfBitmap bitmap = new PdfBitmap(width, height, true);
                if (bitmap.Bitmap == null)
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

                        fpdfview.FPDF_RenderPageBitmapWithMatrix(bitmap.Bitmap, page, matrix, clipping, renderFlag);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("error rendering page", ex);
                }

                return bitmap;
            }
        }

        /// <summary>
        /// 应用开辟内存给Pdfium存储图像, 其可以是应用从数组创建, 也可以是通过图像处理库创建的, 记得需要应用自己管理该内存.
        /// 其相对于<see cref="DrawToPdfBitmap(float, float, int)"/>优势是可以利用已经开辟得内存空间.
        /// </summary>
        /// <param name="imageBufferPointer"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="rotate"></param>
        /// <param name="renderFlag"><see cref="RenderFlags"/>,可以叠加，如`RenderFlags.RenderAnnotations | RenderFlags.RenderForPrinting`</param>
        /// <exception cref="Exception"></exception>
        public void Draw(IntPtr imageBufferPointer, float xScale, float yScale, int renderFlag)
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

        public void Draw(IntPtr imageBufferPointer, int width, bool renderAnnot)
        {
            var scale = width / GetSize().Width;
            Draw(imageBufferPointer, scale, scale, (int)(renderAnnot ? RenderFlags.RenderAnnotations : RenderFlags.None));
        }

        public object GetThumbnail(Size size, PdfDisplayBox box)
        {
            var bitmap = fpdf_thumbnail.FPDFPageGetThumbnailAsBitmap(this.page);
            if (bitmap != null)
                return new PdfBitmap(bitmap);
            //没有生成Pdf自带的缩略图时,我们自己生成页面图像
            var pageSize = GetSize();
            return DrawToPdfBitmap(size.Width / pageSize.Width, size.Height / pageSize.Height, (int)RenderFlags.None);
        }
        #endregion

        #region PageObj

        /// <summary>
        /// 添加对象到页面. 注意添加后原对象被自动释放, 请重新获取
        /// </summary>
        /// <param name="obj"></param>
        public void AppendObj(PdfPageObj obj)
        {
            obj.PageObjTag = 2;
            fpdf_edit.FPDFPageInsertObject(Page, obj.PageObj);
        }
        
        /// <summary>
        /// 从页面移除后需自行释放
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObj(PdfPageObj obj)
        {
            fpdf_edit.FPDFPageRemoveObject(Page, obj.PageObj);
        }

        public int GetObjCount()
        {
            return fpdf_edit.FPDFPageCountObjects(Page);
        }

        public PdfPageObj GetObj(int index)
        {
            var obj = fpdf_edit.FPDFPageGetObject(Page, index);
            if (obj != null)
            {
                var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                {
                    return new PdfPageTextObj(obj) { Index = index };
                }
                else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                {
                    return new PdfPageImageObj(obj) { Index = index };
                }
                else if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                {
                    return new PdfPagePathObj(obj) { Index = index };
                }
            }
            return null;
        }

        public PdfPageObj[] GetAllObj()
        {
            var objectCount = fpdf_edit.FPDFPageCountObjects(Page);
            if (objectCount > 0)
            {
                var pdfPageObjs = new PdfPageObj[objectCount];

                for (var index = 0; index < objectCount; index++)
                {
                    var obj = GetObj(index);
                    if (obj != null)
                        pdfPageObjs[index] = obj;
                }
                return pdfPageObjs;
            }
            return null;
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
                Document = null;
            }
        }
    }
}