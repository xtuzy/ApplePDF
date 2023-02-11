namespace ApplePDF.PdfKit.Annotation
{
    /// <summary>
    /// The type of annotation, such as circle, text, or ink.
    /// 由于type可以自定义, 因此可能是unknow
    /// </summary>
    public enum PdfAnnotationSubtype
    {
        Text,
        Link,
        FreeText,
        Line,
        /// <summary>
        /// 方形
        /// </summary>
        Square,
        Circle,
        Highlight,
        Underline,
        Squiggly,
        StrikeOut,
        Ink,
        /// <summary>
        /// 图章
        /// </summary>
        Stamp,
        Popup,
        Widget,
        /// <summary>
        /// 多边形
        /// </summary>
        Polygon,
        /// <summary>
        /// 折线
        /// </summary>
        Polyline,
        Unknow
    }
}
