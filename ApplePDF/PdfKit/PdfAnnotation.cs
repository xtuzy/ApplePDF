
using ApplePDF.PdfKit.Annotation;
using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ApplePDF.PdfKit
{
    public abstract class PdfAnnotation : IPdfAnnotation, IDisposable
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
                /// <summary>
                /// 注释的类型, 可是自定义字符串
                /// </summary>
                public const string kSubtype = "Subtype";
                public const string kRect = "Rect";
                public const string kContents = "Contents";
                /// <summary>
                /// 所属页面
                /// </summary>
                public const string kP = "P";
                public const string kNM = "NM";
                /// <summary>
                /// 修改时间
                /// </summary>
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
        }

        public FpdfAnnotationT Annotation { get; protected set; }
        public int Index { get; protected set; }
        public PdfPage Page { get; protected set; }
        public PdfAnnotationSubtype AnnotationType { get; protected set; }

        /// <summary>
        /// The annot 's edge box?
        /// I don't know the different of AnnotBox and Annot Points,maybe add point will auto update box?
        /// </summary>
        public PdfRectangleF AnnotBox
        {
            get
            {
                var position = new FS_RECTF_();
                var success = fpdf_annot.FPDFAnnotGetRect(Annotation, position) == 1;
                if (success)
                    return PdfRectangleF.FromLTRB(position.Left, position.Top, position.Right, position.Bottom);
                else
                {
                    Debug.WriteLine($"{this.GetType()}:Get AnnotBox fail.");
                    return PdfRectangleF.Empty;
                }
            }

            set
            {
                var position = new FS_RECTF_();
                position.Left = value.Left;
                position.Top = value.Top;
                position.Right = value.Right;
                position.Bottom = value.Bottom;
                var success = fpdf_annot.FPDFAnnotSetRect(Annotation, position) == 1;
                if (!success)
                {
                    throw new Exception("Set AnnotBox fail.");
                }
            }
        }

        #region Color

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

        #endregion

        #region Obj
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
            if (fpdf_annot.FPDFAnnotAppendObject(Annotation, obj.PageObj) == 1)
            {
                var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
                obj.Index = objectCount-1;
                return true;
            }
            else
                return false;
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
        /// 移除使用index，没有直接从obj判断index的api，因此其需要根据获取obj的先后判断， 但移除后会造成index变化，因此建议之后重新获取全部obj
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected bool RemoveObjOfAnnot(PdfPageObj obj, List<PdfPageObj> sourceObjs)
        {
            if (sourceObjs.Contains(obj))
            {
                if (fpdf_annot.FPDFAnnotRemoveObject(Annotation, obj.Index) == 1)
                {
                    //之后的注释的index需要更新
                    var index = sourceObjs.IndexOf(obj);
                    for (int i = index + 1; i < sourceObjs.Count; i++)
                        sourceObjs[i].Index = i - 1;
                    sourceObjs.RemoveAt(index);
                    obj.PageObjTag = 1;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                throw new ArgumentException("移除的对象不在从注释获取的对象列表内.");
            }
        }

        /// <summary>
        /// 注释实际Obj数量,不是<see cref="PdfPageObjs"/>的数量, 主要用于在添加新的Obj时获取Index.
        /// </summary>
        /// <returns></returns>
        public int GetObjCount()
        {
            return fpdf_edit.FPDFPageCountObjects(Page.Page);
        }

        protected List<PdfPageObj> GetAllObj()
        {
            var objectCount = fpdf_annot.FPDFAnnotGetObjectCount(Annotation);
            if (objectCount > 0)
            {
                var pdfPageObjs = new List<PdfPageObj>();
                //此处分析注释数据时只当注释只有一个文本和图像对象
                for (int objIndex = 0; objIndex < objectCount; objIndex++)
                {
                    var obj = fpdf_annot.FPDFAnnotGetObject(Annotation, objIndex);
                    if (obj != null)
                    {
                        var objectType = fpdf_edit.FPDFPageObjGetType(obj);
                        if (objectType == (int)PdfPageObjectTypeFlag.TEXT)
                        {
                            pdfPageObjs.Add(new PdfPageTextObj(obj) { Index = objIndex });
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.IMAGE)
                        {
                            pdfPageObjs.Add(new PdfPageImageObj(obj) { Index = objIndex });
                        }
                        else if (objectType == (int)PdfPageObjectTypeFlag.PATH)
                        {
                            pdfPageObjs.Add(new PdfPagePathObj(obj) { Index = objIndex });
                        }
                    }
                }
                return pdfPageObjs;
            }

            if (objectCount == 0)
            {
                // 测试mytest_4_freetextannotation.pdf时,为0时貌似也可能正确,这个注释好像是不显示的
                return null;
            }

            return null;
        }

        #endregion

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

        #region Key

        public string GetStringValueFromKey(string key = PdfAnnotation.Constant.CommonKey.kContents)
        {
            // 先尝试使用StringValue获取文本，当返回2时就是没有
            ushort[] buffer = new ushort[1];
            var resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, key, ref buffer[0], (uint)0);
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
                resultBytesLength = fpdf_annot.FPDFAnnotGetStringValue(Annotation, key, ref buffer[0], (uint)buffer.Length);
                unsafe
                {
                    fixed (ushort* dataPtr = &buffer[0])
                        return new string((char*)dataPtr, 0, (int)(resultBytesLength - 2) / 2);//返回的result长度会有结束符,貌似占用两个byte长度,所以减去,ushort占用两个byte,所以除以2
                }
            }
        }

        public bool SetStringValueForKey(string text, string key = PdfAnnotation.Constant.CommonKey.kContents)
        {
            //Set Text
            //string to ushort 参考:https://stackoverflow.com/a/274207/13254773
            var bytes = Encoding.Unicode.GetBytes(text);
            ushort[] value = new ushort[text.Length];
            Buffer.BlockCopy(bytes, 0, value, 0, bytes.Length);

            //设置注释本身的StringValue
            var success = fpdf_annot.FPDFAnnotSetStringValue(Annotation, key, ref value[0]) == 1;
            if (!success)
            {
                Debug.WriteLine($"{TAG}:Set string value fail");
                return false;
            }
            return true;
        }

        #endregion

        #region AttachmentPoint

        public void SetQuadPoint(PdfRectangleF rect)
        {
            var success = fpdf_annot.FPDFAnnotAppendAttachmentPoints(Annotation, new FS_QUADPOINTSF()
            {
                X1 = rect.Left,
                Y1 = rect.Top,
                X2 = rect.Right,
                Y2 = rect.Top,
                X3 = rect.Left,
                Y3 = rect.Bottom,
                X4 = rect.Right,
                Y4 = rect.Bottom
            }) == 1;
        }

        public PdfRectangleF? GetQuadPoints()
        {
            var count = (int)fpdf_annot.FPDFAnnotCountAttachmentPoints(Annotation);
            var success = count != 0;
            if (!success) Debug.WriteLine("No AttachmentPoints");
            else
            {
                if (count > 1)
                {
                    return null;
                    throw new NotImplementedException("获取到的QuadPoints不止一个, 因为设置时只能设置一个, 因此此处只允许获取一个的情况.");
                }
                else
                {
                    var point = new FS_QUADPOINTSF();

                    success = fpdf_annot.FPDFAnnotGetAttachmentPoints(Annotation, (ulong)0, point) == 1;
                    if (!success)
                    {
                        Debug.WriteLine("Get AttachmentPoints fail");
                        return null;
                    }
                    return PdfRectangleF.FromLTRB(point.X1, point.Y1, point.X4, point.Y4);
                }
            }
            return null;
        }

        #endregion

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