using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace PdfKit
{
    public interface IPdfAnnotation
    {
        PdfAction Action { get; set; }
        UITextAlignment Alignment { get; set; }
        bool AllowsToggleToOff { get; set; }
        NSDictionary AnnotationKeyValues { get; }
        PdfAnnotationKey AnnotationType { get; set; }
        UIColor BackgroundColor { get; set; }
        PdfBorder Border { get; set; }
        CGRect Bounds { get; set; }
        PdfWidgetCellState ButtonWidgetState { get; set; }
        string ButtonWidgetStateString { get; set; }
        string Caption { get; set; }
        string[] Choices { get; set; }
        IntPtr ClassHandle { get; }
        UIColor Color { get; set; }
        bool Comb { get; set; }
        string Contents { get; set; }
        PdfDestination Destination { get; set; }
        PdfLineStyle EndLineStyle { get; set; }
        CGPoint EndPoint { get; set; }
        string FieldName { get; set; }
        UIFont Font { get; set; }
        UIColor FontColor { get; set; }
        bool HasAppearanceStream { get; }
        bool Highlighted { get; set; }
        PdfTextAnnotationIconType IconType { get; set; }
        UIColor InteriorColor { get; set; }
        bool IsPasswordField { get; }
        bool ListChoice { get; set; }
        PdfMarkupType MarkupType { get; set; }
        nint MaximumLength { get; set; }
        NSDate ModificationDate { get; set; }
        PdfAction MouseUpAction { get; set; }
        bool Multiline { get; set; }
        bool Open { get; set; }
        PdfPage Page { get; set; }
        UIBezierPath[] Paths { get; }
        PdfAnnotation Popup { get; set; }
        CGPoint[] QuadrilateralPoints { get; set; }
        bool RadiosInUnison { get; set; }
        bool ReadOnly { get; set; }
        bool ShouldDisplay { get; set; }
        bool ShouldPrint { get; set; }
        string StampName { get; set; }
        PdfLineStyle StartLineStyle { get; set; }
        CGPoint StartPoint { get; set; }
        string ToolTip { get; }
        string Type { get; set; }
        NSUrl Url { get; set; }
        string UserName { get; set; }
        string[] Values { get; set; }
        PdfWidgetControlType WidgetControlType { get; set; }
        string WidgetDefaultStringValue { get; set; }
        string WidgetFieldType { get; set; }
        string WidgetStringValue { get; set; }

        void AddBezierPath(UIBezierPath path);
        //NSObject Copy(NSZone zone);
        //void Draw(PdfDisplayBox box, CGContext context);
        //void EncodeTo(NSCoder encoder);
       // T GetValue<T>(PdfAnnotationKey key) where T : class, INativeObject;
        //void RemoveAllAppearanceStreams();
        //void RemoveBezierPath(UIBezierPath path);
        //void RemoveValue(PdfAnnotationKey key);
        //bool SetValue(bool boolean, PdfAnnotationKey key);
        //bool SetValue(CGRect rect, PdfAnnotationKey key);
        //bool SetValue(string str, PdfAnnotationKey key);
        //bool SetValue<T>(T value, PdfAnnotationKey key) where T : class, INativeObject;
    }
}