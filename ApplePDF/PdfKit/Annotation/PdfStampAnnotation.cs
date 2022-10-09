namespace ApplePDF.PdfKit.Annotation
{
    using PDFiumCore;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// 图章注释,可以包含文本和图片.
    /// </summary>
    public class PdfStampAnnotation : PdfAnnotation, IFillColorAnnotation, IStrokeColorAnnotation
    {
        private const string TAG = nameof(PdfStampAnnotation);

        /// <summary>
        /// 创建新的Stamp注释
        /// </summary>
        public PdfStampAnnotation()
            : base(PdfAnnotationSubtype.Stamp)
        {
        }

        internal PdfStampAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
            : base(page, annotation, type, index)
        {
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);

            //此处分析注释数据时只当注释只有一个文本和图像对象
            for (int objIndex = 0; objIndex < objectCount; objIndex++)
            {
                var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, objIndex);
                if (obj != null)
                {
                    var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                    if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                    {
                        // 颜色
                        uint R = 0;
                        uint G = 0;
                        uint B = 0;
                        uint A = 0;
                        var success = fpdf_edit.FPDFPageObjGetFillColor(obj, ref R, ref G, ref B, ref A) == 1;
                        if (success)
                        {
                            this.FillColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        }
                        else
                        {
                            Debug.WriteLine($"{TAG}:No fill color");
                        }

                        success = fpdf_edit.FPDFPageObjGetStrokeColor(obj, ref R, ref G, ref B, ref A) == 1;

                        if (success)
                        {
                            this.StrokeColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        }
                        else
                        {
                            Debug.WriteLine($"{TAG}:No stroke color");
                        }

                        //字体
                        using (var font = new PdfFont(fpdf_edit.FPDFTextObjGetFont(obj)))
                        {
                            TextFont = font.Name;
                        }

                        float textSize = default;
                        if (fpdf_edit.FPDFTextObjGetFontSize(obj, ref textSize) == 1)
                        {
                            TextSize = textSize;
                        }

                        // 尝试从文本对象获取Text
                        var buffer = new ushort[100];
                        var result = fpdf_edit.FPDFTextObjGetText(obj, page.TextPage, ref buffer[0], (uint)buffer.Length);
                        if (result != 0)
                        {
                            unsafe
                            {
                                fixed (ushort* dataPtr = &buffer[0])
                                {
                                    this.Text = new string((char*)dataPtr, 0, (int)result);
                                }
                            }
                        }
                    }
                    else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                    {
                        // TODO:Image stamp
                    }
                }
            }

            if (objectCount == 0)
            {
                // 测试mytest_4_freetextannotation.pdf时,为0时貌似也可能正确,这个注释好像是不显示的
            }
        }

        /// <summary>
        /// 文本颜色.
        /// </summary>
        public Color? FillColor { get; set; }

        /// <summary>
        /// 文本颜色.
        /// </summary>
        public Color? StrokeColor { get; set; }

        public string? Text { get; set; }

        /// <summary>
        /// Default is 12.
        /// </summary>
        public float TextSize { get; set; }

        /// <summary>
        /// Default is Arial.
        /// </summary>
        public string TextFont { get; set; }

        /// <summary>
        /// 指向图像.大小需和<see cref="PdfAnnotation.AnnotBox"/>一样.
        /// </summary>
        public IntPtr? ImageBuffer { get; set; }

        internal override void AddToPage(PdfPage page)
        {
            base.AddToPage(page);

            //添加文本
            if (this.Text != null)
            {
                // 创建文本对象
                var textObj = fpdf_edit.FPDFPageObjNewTextObj(Page.Document.Document, TextFont, TextSize);

                // 设置文本
                // string to ushort 参考:https://stackoverflow.com/a/274207/13254773
                var bytes = Encoding.Unicode.GetBytes(this.Text);
                ushort[] value = new ushort[this.Text.Length];
                Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);
                var success = fpdf_edit.FPDFTextSetText(textObj, ref value[0]) == 1;
                if (!success)
                {
                    throw new NotImplementedException($"{TAG}:Fail to set text to textobj");
                }

                // text fill color
                if (this.FillColor != null)
                {
                    if (fpdf_edit.FPDFPageObjSetFillColor(textObj, this.FillColor.Value.R, this.FillColor.Value.G, this.FillColor.Value.B, this.FillColor.Value.A) == 0)
                    {
                        Debug.WriteLine($"{TAG}:Fail to set fill color to textobj");
                    }
                }

                // text stroke color
                if (this.StrokeColor != null)
                {
                    if (fpdf_edit.FPDFPageObjSetStrokeColor(textObj, this.StrokeColor.Value.R, this.StrokeColor.Value.G, this.StrokeColor.Value.B, this.StrokeColor.Value.A) == 0)
                    {
                        Debug.WriteLine($"{TAG}:Fail to set stroke color to textobj");
                    }
                }

                // 文本对象添加到注释
                if (fpdf_annot.FPDFAnnotAppendObject(Annotation, textObj) == 0)
                {
                    Debug.WriteLine($"{TAG}:Fail to append textObj to annotation");
                }
            }

            //添加图像
            if (ImageBuffer != null)
            {
                // 我的理解是,这个方法让Pdfium的Bitmap对象的指针指向现成的图片,从而可以直接把图片写到Pdf
                var bitmap = fpdfview.FPDFBitmapCreateEx((int)AnnotBox.Width, (int)AnnotBox.Height, (int)FPDFBitmapFormat.BGRA, ImageBuffer.Value, (int)AnnotBox.Width * 4);
                if (bitmap == null)
                {
                    throw new Exception($"{TAG}:Failed to create a pdf bitmap");
                }
                var imageObj = fpdf_edit.FPDFPageObjNewImageObj(Page.Document.Document);
                if (fpdf_edit.FPDFImageObjSetBitmap(Page.Page, Page.PageIndex, imageObj, bitmap) == 0)
                {
                    Debug.WriteLine($"{TAG}:Fail to set bitmap to imageObj");
                }
                else
                {
                    // 图像对象添加到注释
                    if (fpdf_annot.FPDFAnnotAppendObject(Annotation, imageObj) == 0)
                    {
                        Debug.WriteLine($"{TAG}:Fail to append imageObj to annotation");
                    }
                }
            }
        }

        /// <summary>
        /// 替代注释的文本.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>替代成功返回true</returns>
        /// <exception cref="ArgumentNullException"><see cref="Annotation"/> must not null</exception>
        public bool Instead(string text)
        {
            if (this.Annotation == null)
            {
                throw new ArgumentNullException($"{TAG}:Annotation is null, if you use a new PdfAnnotation, please load AddToPage before load instead");
            }

            this.Text = text;

            // 查找文本对象
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount == 1)
            {
                var textObj = fpdf_annot.FPDFAnnotGetObject(Annotation, 0);
                if (textObj != null)
                {
                    var objectType = fpdf_edit.FPDFPageObjGetType(textObj);
                    if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                    {
                        //设置文本
                        //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
                        var bytes = Encoding.Unicode.GetBytes(Text);
                        ushort[] value = new ushort[Text.Length];
                        Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);
                        var success = fpdf_edit.FPDFTextSetText(textObj, ref value[0]) == 1;
                        //更新到注释
                        if (fpdf_annot.FPDFAnnotUpdateObject(Annotation, textObj) != 0)
                        {
                            return true;
                        }
                        else
                            Debug.WriteLine($"{TAG}:Fail to set text to textobj");
                    }
                    else
                        Debug.WriteLine($"{TAG}:First object in annt is not textobj");
                }
                else
                    Debug.WriteLine($"{TAG}:First object in annt is null");
            }
            else
                Debug.WriteLine($"{TAG}:Object count not equal 1 in this annotation, so i don't know instead which one");
            return false;
        }
    }
}
