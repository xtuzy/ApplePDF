
using ApplePDF.PdfKit.Annotation;
using PDFiumCore;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public abstract class PdfAnnotation : IDisposable
    {
        const string TAG = nameof(PdfAnnotation);

        /// <summary>
        /// 参考:https://github.com/chromium/pdfium/blob/main/constants/annotation_common.h.
        /// Pdfium测试中获取和设置文本时使用了<see cref="Constant.CommonKey.KContents"/>, 我对照Pdf Reference 1.7 的8.4节搬运了些.
        /// 
        /// </summary>
        public class Constant
        {
            public class CommonKey
            {
                public const string kType = "Type";
                public const string kSubtype = "Subtype";
                public const string kRect = "Rect";
                public const string kContents = "Contents";
                public const string kP = "P";
                public const string kNM = "NM";
                public const string kM = "M";
                public const string kF = "F";
                public const string kAP = "AP";
                public const string kAS = "AS";
                public const string kBorder = "Border";
                public const string kC = "C";
                public const string kStructParent = "StructParent";
                public const string kOC = "OC";
            }

            public enum AppearenceMode
            {
                /// <summary>
                /// The normal appearance shall be used when the annotation is not interacting with the user. This
                /// appearance is also used for printing the annotation.
                /// </summary>
                FPDF_ANNOT_APPEARANCEMODE_NORMAL,
                /// <summary>
                /// The rollover appearance shall be used when the user moves the cursor into the annotation's active area
                /// without pressing the mouse button.
                /// </summary>
                FPDF_ANNOT_APPEARANCEMODE_ROLLOVER,
                /// <summary>
                /// The down appearance shall be used when the mouse button is pressed or held down within the
                /// annotation's active area.
                /// </summary>
                FPDF_ANNOT_APPEARANCEMODE_DOWN,
                FPDF_ANNOT_APPEARANCEMODE_COUNT,
            }
        }

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
            if (AnnotBox != PdfRectangleF.Empty)
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

        public string GetAppearenceStr(Constant.AppearenceMode appearenceMode = Constant.AppearenceMode.FPDF_ANNOT_APPEARANCEMODE_NORMAL)
        {
            ushort[] buffer = new ushort[1];
            var resultBytesLength = fpdf_annot.FPDFAnnotGetAP(Annotation, (int)appearenceMode, ref buffer[0], (uint)0);
            if (resultBytesLength == 0)
            {
                throw new NotImplementedException($"{TAG}:Fail to get appearence string, no reason return.");
            }
            else if (resultBytesLength == 2)
            {
                Debug.WriteLine($"{TAG}:No appearence string in this annotation");
                return string.Empty;
            }
            else
            {
                buffer = new ushort[resultBytesLength];
                resultBytesLength = fpdf_annot.FPDFAnnotGetAP(Annotation, (int)Constant.AppearenceMode.FPDF_ANNOT_APPEARANCEMODE_NORMAL, ref buffer[0], (uint)buffer.Length * 2);//ushort长度转bytes长度
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        return new string((char*)dataPtr, 0, (int)(resultBytesLength - 2) / 2);//bytes长度转ushort长度
                }
            }
        }

        public bool SetAppearenceStr(string text, Constant.AppearenceMode appearenceMode = Constant.AppearenceMode.FPDF_ANNOT_APPEARANCEMODE_NORMAL)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            ushort[] value = new ushort[text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);

            //设置注释本身的StringValue
            var success = fpdf_annot.FPDFAnnotSetAP(Annotation, (int)appearenceMode, ref value[0]) == 1;
            if (!success)
            {
                Debug.WriteLine($"{TAG}:Set appearence string fail");
                return false;
            }
            return true;
        }

        protected string GetStringValue()
        {
            // 先尝试使用StringValue获取文本，当返回2时就是没有
            ushort[] buffer = new ushort[1];
            var resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, PdfAnnotation.Constant.CommonKey.kContents, ref buffer[0], (uint)0);
            if (resultBytesLength == 0)
            {
                throw new NotImplementedException($"{TAG}:Create PdfFreeTextAnnotation fail, no reason return.");
            }
            else if (resultBytesLength == 2)
            {
                Debug.WriteLine($"{TAG}:By FPDFAnnotGetStringValue() create PdfFreeTextAnnotation fail, because don't find text content in this annotation, next will try use textObject.");
                return string.Empty;
            }
            else
            {
                buffer = new ushort[resultBytesLength];
                resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, PdfAnnotation.Constant.CommonKey.kContents, ref buffer[0], (uint)buffer.Length);
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        return new string((char*)dataPtr, 0, (int)(resultBytesLength - 2) / 2);//返回的result长度会有结束符,貌似占用两个byte长度,所以减去,ushort占用两个byte,所以除以2
                }
            }
        }

        protected bool SetStringValue(string text)
        {
            //Set Text
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(text);
            ushort[] value = new ushort[text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);

            //设置注释本身的StringValue
            var success = fpdf_annot.FPDFAnnotSetStringValue(Annotation, PdfAnnotation.Constant.CommonKey.kContents, ref value[0]) == 1;
            if (!success)
            {
                Debug.WriteLine($"{TAG}:Set string value fail");
                return false;
            }
            return true;
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