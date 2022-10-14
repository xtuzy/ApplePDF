
using ApplePDF.PdfKit.Annotation;
using PDFiumCore;
using System;
using System.Diagnostics;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public abstract class PdfAnnotation : IDisposable
    {
        const string TAG = nameof(PdfAnnotation);

        //参考:https://github.com/LibreOffice/core/blob/92cba30d5ce45e4f4a9516a80c9fe9915add6905/include/vcl/filter/PDFiumLibrary.hxx
        public const string ConstDictionaryKeyTitle = "T";
        public const string ConstDictionaryKeyContents = "Contents";
        public const string ConstDictionaryKeyPopup = "Popup";
        public const string ConstDictionaryKeyModificationDate = "M";
        public const string ConstDictionaryKeyInteriorColor = "IC";

        internal PdfAnnotation(PdfPage page, FpdfAnnotationT annotation, PdfAnnotationSubtype type, int index)
        {
            this.Page = page;
            this.Annotation = annotation;
            this.AnnotationType = type;
            this.Index = index;

            // 位置
            var position = new FS_RECTF_();
            var success = fpdf_annot.FPDFAnnotGetRect(Annotation, position) == 1;
            if (success)
                AnnotBox = PdfRectangleF.FromLTRB(position.Left, position.Top, position.Right, position.Bottom);
            else
                Debug.WriteLine($"{this.GetType()}:Get AnnotBox fail.");
        }

        /// <summary>
        /// 为创建新注释
        /// </summary>
        /// <param name="type"></param>
        public PdfAnnotation(PdfAnnotationSubtype type)
        {
            AnnotationType = type;
        }

        public FpdfAnnotationT Annotation { get; protected set; }
        public int Index { get; protected set; }
        public PdfPage Page { get; protected set; }
        public PdfAnnotationSubtype AnnotationType { get; protected set; }
        #region 用户设置

        /// <summary>
        /// The annot 's edge box?
        /// I don't know the different of AnnotBox and Annot Points,maybe add point will auto update box?
        /// </summary>
        public PdfRectangleF AnnotBox { get; set; } = PdfRectangleF.Empty;

        #endregion 用户设置
        internal virtual void AddToPage(PdfPage page)
        {
            this.Page = page;

            // 创建注释
            var annot = fpdf_annot.FPDFPageCreateAnnot(page.Page, (int)this.AnnotationType);
            if (annot == null)
            {
                throw new NotImplementedException($"Cant't create new {AnnotationType} annotation");
            }
            this.Annotation = annot;
            var index = fpdf_annot.FPDFPageGetAnnotIndex(page.Page, this.Annotation);
            this.Index = index;
            bool success = false;

            // 位置
            if(AnnotBox != PdfRectangleF.Empty)
            {
                success = fpdf_annot.FPDFAnnotSetRect(Annotation, new FS_RECTF_()
                {
                    Left = AnnotBox.Left,
                    Top = AnnotBox.Top,
                    Right = AnnotBox.Right,
                    Bottom = AnnotBox.Bottom
                }) == 1;
                if (!success) new NotImplementedException($"{this.GetType()}:Set AnnotBox fail");

            }

            // Flag?
            //success = fpdf_annot.FPDFAnnotSetFlags(Annotation, 4) == 1;
            //if (!success) new NotImplementedException($"{this.GetType()}:Set flag fail");
        }

        /// <summary>
        /// 请勿使用,仅为测试
        /// </summary>
        /// <returns></returns>
        public (Color? AnnotColor, Color? FillColor, Color? StrokeColor) TryGetColor(PdfPageObjectTypeFlag objType)
        {
            var colors = GetFillAndStrokeColor(objType);
            return (GetAnnotColor(), colors.FillColor, colors.StrokeColor);
        }

        /// <summary>
        /// If return null, you need use <see cref="PdfPageObj.GetFillColor"/> or <see cref="PdfPageObj.GetStrokeColor"/> to get color.
        /// </summary>
        /// <returns></returns>
        protected Color? GetAnnotColor()
        {
            // 获取颜色
            uint R = 0;
            uint G = 0;
            uint B = 0;
            uint A = 0;
            var success = fpdf_annot.FPDFAnnotGetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, ref R, ref G, ref B, ref A) == 1;
            if (success)
                return System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
            else
                return null;
        }

        /// <summary>
        /// If parameter is not null and return false, you need use <see cref="PdfPageObj.SetFillColor(Color?)"/> or <see cref="PdfPageObj.SetStrokeColor(Color?)"/> to set color.
        /// </summary>
        /// <param name="annotColor"></param>
        /// <returns>Return true if success</returns>
        protected bool SetAnnotColor(Color? annotColor)
        {
            // 设置颜色,我们不管其它软件是否使用对象来设置颜色,我们用最简单的方式
            if (annotColor != null)
            {
                return fpdf_annot.FPDFAnnotSetColor(Annotation, FPDFANNOT_COLORTYPE.FPDFANNOT_COLORTYPE_Color, annotColor.Value.R, annotColor.Value.G, annotColor.Value.B, annotColor.Value.A) == 1;
            }
            return false;
        }

        /// <summary>
        /// For FPDFPageObjGetFillColor() and FPDFPageObjGetStrokeColor(). 仅取第一个obj的颜色
        /// </summary>
        /// <returns></returns>
        private (Color? FillColor, Color? StrokeColor) GetFillAndStrokeColor(PdfPageObjectTypeFlag objType)
        {
            Color? FillColor = null; Color? StrokeColor = null;

            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);

            //此处分析注释数据时只当注释只有一个文本和图像对象
            for (int objIndex = 0; objIndex < objectCount; objIndex++)
            {
                var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, objIndex);
                if (obj != null)
                {
                    var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                    if (objectType == (int)objType)
                    {
                        // 颜色
                        uint R = 0;
                        uint G = 0;
                        uint B = 0;
                        uint A = 0;
                        var success = fpdf_edit.FPDFPageObjGetFillColor(obj, ref R, ref G, ref B, ref A) == 1;
                        if (success)
                        {
                            FillColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        }
                        else
                        {
                            Debug.WriteLine($"{nameof(PdfFreeTextAnnotation)}:No fill color");
                        }

                        success = fpdf_edit.FPDFPageObjGetStrokeColor(obj, ref R, ref G, ref B, ref A) == 1;

                        if (success)
                        {
                            StrokeColor = System.Drawing.Color.FromArgb((int)A, (int)R, (int)G, (int)B);
                        }
                        else
                        {
                            Debug.WriteLine($"{nameof(PdfFreeTextAnnotation)}:No stroke color");
                        }

                        break;//识别第一个
                    }
                }
            }
            return (FillColor, StrokeColor);
        }

        /// <summary>
        /// Append a new <see cref="PdfPageObj"/> to <see cref="PdfAnnotation"/>.
        /// Support <see cref="PdfInkAnnotation"/> and <see cref="PdfStampAnnotation"/>. Because <see cref="fpdf_annot.FPDFAnnotAppendObject"/> only support them.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Return true if success</returns>
        protected bool AppendObjToAnnot(PdfPageObj obj)
        {
            // 对象添加到注释
            obj.PageObjTag = 2;
            return fpdf_annot.FPDFAnnotAppendObject(Annotation, obj.PageObj) == 1;
        }

        /// <summary>
        /// Update content of <see cref="PdfPageObj"/> after you set content of obj, such as <see cref="PdfPageObj.SetFillColor(Color?)"/>.
        /// Support <see cref="PdfInkAnnotation"/> and <see cref="PdfStampAnnotation"/>. Because <see cref="fpdf_annot.FPDFAnnotUpdateObject"/> only support them.
        /// </summary>
        /// <param name="obj"></param>
        protected bool UpdateObjOfAnnot(PdfPageObj obj)
        {
            // 文本对象添加到注释
            return fpdf_annot.FPDFAnnotUpdateObject(Annotation, obj.PageObj) == 1;
        }

        /// <summary>
        /// Close this annot and release resource.
        /// </summary>
        public void Dispose() => Dispose(true);

        bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                if (Annotation != null)
                {
                    fpdf_annot.FPDFPageCloseAnnot(Annotation);
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            Page = null;
            Annotation = null;
            _disposed = true;
        }
    }
}