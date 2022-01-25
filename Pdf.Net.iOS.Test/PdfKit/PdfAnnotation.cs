#region Assembly Xamarin.iOS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065
// location unknown
// Decompiled with ICSharpCode.Decompiler 6.1.0.5902
#endregion

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UIKit;

namespace PdfKit
{
    //
    // Summary:
    //     Notes, highlights, or other additions to a PDF file.
    //
    // Remarks:
    //     To be added.
    [Register("PDFAnnotation", true)]
    [Introduced(PlatformName.iOS, 11, 0, PlatformArchitecture.All, null)]
    public class PdfAnnotation : NSObject, INSCoding, INativeObject, IDisposable, INSCopying, IPdfAnnotation
    {
        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        public PdfAnnotationKey AnnotationType
        {
            get
            {
                throw null;
            }
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Mac(10, 13)]
        public CGPoint[] QuadrilateralPoints
        {
            [NullableContext(1)]
            get
            {
                throw null;
            }
            [NullableContext(1)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Obsolete("Empty stub (not a public API on iOS).")]
        public virtual string? ToolTip
        {
            [CompilerGenerated]
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Obsolete("Empty stub (not a public API).")]
        public virtual PdfAction? MouseUpAction
        {
            [CompilerGenerated]
            get
            {
                throw null;
            }
            [CompilerGenerated]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     The handle for this class.
        //
        // Value:
        //     The pointer to the Objective-C class.
        //
        // Remarks:
        //     Each Xamarin.iOS class mirrors an unmanaged Objective-C class. This value contains
        //     the pointer to the Objective-C class, it is similar to calling objc_getClass
        //     with the object name.
        public override IntPtr ClassHandle
        {
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfAction? Action
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("action", ArgumentSemantic.Retain)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setAction:", ArgumentSemantic.Retain)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual UITextAlignment Alignment
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("alignment", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setAlignment:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool AllowsToggleToOff
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("allowsToggleToOff")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setAllowsToggleToOff:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual NSDictionary AnnotationKeyValues
        {
            [NullableContext(1)]
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("annotationKeyValues", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual UIColor? BackgroundColor
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("backgroundColor", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setBackgroundColor:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual PdfBorder? Border
        {
            [Export("border")]
            get
            {
                throw null;
            }
            [Export("setBorder:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual CGRect Bounds
        {
            [Export("bounds")]
            get
            {
                throw null;
            }
            [Export("setBounds:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfWidgetCellState ButtonWidgetState
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("buttonWidgetState", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setButtonWidgetState:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string ButtonWidgetStateString
        {
            [NullableContext(1)]
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("buttonWidgetStateString")]
            get
            {
                throw null;
            }
            [NullableContext(1)]
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setButtonWidgetStateString:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string? Caption
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("caption")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setCaption:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string[]? Choices
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("choices", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setChoices:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual UIColor Color
        {
            [NullableContext(1)]
            [Export("color")]
            get
            {
                throw null;
            }
            [NullableContext(1)]
            [Export("setColor:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool Comb
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("hasComb")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setComb:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual string? Contents
        {
            [Export("contents")]
            get
            {
                throw null;
            }
            [Export("setContents:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfDestination? Destination
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("destination", ArgumentSemantic.Retain)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setDestination:", ArgumentSemantic.Retain)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfLineStyle EndLineStyle
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("endLineStyle", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setEndLineStyle:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual CGPoint EndPoint
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("endPoint", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setEndPoint:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string? FieldName
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("fieldName")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setFieldName:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual UIFont? Font
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("font", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setFont:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual UIColor? FontColor
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("fontColor", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setFontColor:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual bool HasAppearanceStream
        {
            [Export("hasAppearanceStream")]
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool Highlighted
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isHighlighted")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setHighlighted:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfTextAnnotationIconType IconType
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("iconType", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setIconType:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
        [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
        public virtual UIColor? InteriorColor
        {
            [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
            [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
            [Export("interiorColor", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
            [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
            [Export("setInteriorColor:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool IsPasswordField
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isPasswordField")]
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool ListChoice
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isListChoice")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setListChoice:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfMarkupType MarkupType
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("markupType", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setMarkupType:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual nint MaximumLength
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("maximumLength")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setMaximumLength:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual NSDate? ModificationDate
        {
            [Export("modificationDate")]
            get
            {
                throw null;
            }
            [Export("setModificationDate:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool Multiline
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isMultiline")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setMultiline:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool Open
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isOpen")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setOpen:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual PdfPage? Page
        {
            [Export("page")]
            get
            {
                throw null;
            }
            [Export("setPage:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual UIBezierPath[]? Paths
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("paths")]
            get
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual PdfAnnotation? Popup
        {
            [Export("popup")]
            get
            {
                throw null;
            }
            [Export("setPopup:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool RadiosInUnison
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("radiosInUnison")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setRadiosInUnison:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual bool ReadOnly
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("isReadOnly")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setReadOnly:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual bool ShouldDisplay
        {
            [Export("shouldDisplay")]
            get
            {
                throw null;
            }
            [Export("setShouldDisplay:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual bool ShouldPrint
        {
            [Export("shouldPrint")]
            get
            {
                throw null;
            }
            [Export("setShouldPrint:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
        [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
        public virtual string? StampName
        {
            [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
            [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
            [Export("stampName")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, 2, PlatformArchitecture.All, null)]
            [Introduced(PlatformName.iOS, 11, 2, PlatformArchitecture.All, null)]
            [Export("setStampName:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfLineStyle StartLineStyle
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("startLineStyle", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setStartLineStyle:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual CGPoint StartPoint
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("startPoint", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setStartPoint:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual string? Type
        {
            [Export("type")]
            get
            {
                throw null;
            }
            [Export("setType:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual NSUrl? Url
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("URL", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setURL:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual string? UserName
        {
            [Export("userName")]
            get
            {
                throw null;
            }
            [Export("setUserName:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string[]? Values
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("values", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setValues:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual PdfWidgetControlType WidgetControlType
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("widgetControlType", ArgumentSemantic.Assign)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setWidgetControlType:", ArgumentSemantic.Assign)]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string? WidgetDefaultStringValue
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("widgetDefaultStringValue")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setWidgetDefaultStringValue:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string WidgetFieldType
        {
            [NullableContext(1)]
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("widgetFieldType")]
            get
            {
                throw null;
            }
            [NullableContext(1)]
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setWidgetFieldType:")]
            set
            {
                throw null;
            }
        }

        //
        // Summary:
        //     To be added.
        //
        // Value:
        //     (More documentation for this node is coming)
        //     This value can be null.
        //
        // Remarks:
        //     To be added.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        public virtual string? WidgetStringValue
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("widgetStringValue")]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setWidgetStringValue:")]
            set
            {
                throw null;
            }
        }

        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        internal virtual IntPtr _QuadrilateralPoints
        {
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("quadrilateralPoints", ArgumentSemantic.Copy)]
            get
            {
                throw null;
            }
            [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
            [Export("setQuadrilateralPoints:", ArgumentSemantic.Copy)]
            set
            {
                throw null;
            }
        }

        [Mac(10, 12)]
        public bool SetValue<T>(T value, PdfAnnotationKey key) where T : class, INativeObject
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   str:
        //     To be added.
        //
        //   key:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Mac(10, 12)]
        public bool SetValue(string str, PdfAnnotationKey key)
        {
            throw null;
        }

        [Mac(10, 12)]
        public T GetValue<T>(PdfAnnotationKey key) where T : class, INativeObject
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Obsolete("Empty stub (not a public API).")]
        public virtual void RemoveAllAppearanceStreams()
        {
            throw null;
        }

        //
        // Summary:
        //     Default constructor, initializes a new instance of this class.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Export("init")]
        public PdfAnnotation()
        {
            throw null;
        }

        //
        // Summary:
        //     A constructor that initializes the object from the data stored in the unarchiver
        //     object.
        //
        // Parameters:
        //   coder:
        //     The unarchiver object.
        //
        // Remarks:
        //     This constructor is provided to allow the class to be initialized from an unarchiver
        //     (for example, during NIB deserialization). This is part of the Foundation.NSCoding
        //     protocol.
        //     If developers want to create a subclass of this object and continue to support
        //     deserialization from an archive, they should implement a constructor with an
        //     identical signature: taking a single parameter of type Foundation.NSCoder and
        //     decorate it with the [Export("initWithCoder:"] attribute declaration.
        //     The state of this object can also be serialized by using the companion method,
        //     EncodeTo.
        [NullableContext(1)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [DesignatedInitializer]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Export("initWithCoder:")]
        public PdfAnnotation(NSCoder coder)
        {
            throw null;
        }

        //
        // Summary:
        //     Constructor to call on derived classes to skip initialization and merely allocate
        //     the object.
        //
        // Parameters:
        //   t:
        //     Unused sentinel value, pass NSObjectFlag.Empty.
        //
        // Remarks:
        //     This constructor should be called by derived classes when they completely construct
        //     the object in managed code and merely want the runtime to allocate and initialize
        //     the NSObject. This is required to implement the two-step initialization process
        //     that Objective-C uses, the first step is to perform the object allocation, the
        //     second step is to initialize the object. When developers invoke the constructor
        //     that takes the NSObjectFlag.Empty they take advantage of a direct path that goes
        //     all the way up to NSObject to merely allocate the object's memory and bind the
        //     Objective-C and C# objects together. The actual initialization of the object
        //     is up to the developer.
        //     This constructor is typically used by the binding generator to allocate the object,
        //     but prevent the actual initialization to take place. Once the allocation has
        //     taken place, the constructor has to initialize the object. With constructors
        //     generated by the binding generator this means that it manually invokes one of
        //     the "init" methods to initialize the object.
        //     It is the developer's responsibility to completely initialize the object if they
        //     chain up using the NSObjectFlag.Empty path.
        //     In general, if the developer's constructor invokes the NSObjectFlag.Empty base
        //     implementation, then it should be calling an Objective-C init method. If this
        //     is not the case, developers should instead chain to the proper constructor in
        //     their class.
        //     The argument value is ignored and merely ensures that the only code that is executed
        //     is the construction phase is the basic NSObject allocation and runtime type registration.
        //     Typically the chaining would look like this:
        //     //
        //     // The NSObjectFlag merely allocates the object and registers the
        //     // C# class with the Objective-C runtime if necessary, but no actual
        //     // initXxx method is invoked, that is done later in the constructor
        //     //
        //     // This is taken from Xamarin.iOS's source code:
        //     //
        //     [Export ("initWithFrame:")]
        //     public UIView (System.Drawing.RectangleF frame) : base (NSObjectFlag.Empty)
        //     {
        //     // Invoke the init method now.
        //     	var initWithFrame = new Selector ("initWithFrame:").Handle;
        //     	if (IsDirectBinding)
        //     		Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend_CGRect (this.Handle, initWithFrame,
        //     frame);
        //     	else
        //     		Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_CGRect (this.SuperHandle,
        //     initWithFrame, frame);
        //     }
        [NullableContext(1)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected PdfAnnotation(NSObjectFlag t)
        {
            throw null;
        }

        //
        // Summary:
        //     A constructor used when creating managed representations of unmanaged objects;
        //     Called by the runtime.
        //
        // Parameters:
        //   handle:
        //     Pointer (handle) to the unmanaged object.
        //
        // Remarks:
        //     This constructor is invoked by the runtime infrastructure (ObjCRuntime.Runtime.GetNSObject(System.IntPtr))
        //     to create a new managed representation for a pointer to an unmanaged Objective-C
        //     object. Developers should not invoke this method directly, instead they should
        //     call the GetNSObject method as it will prevent two instances of a managed object
        //     to point to the same native object.
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal PdfAnnotation(IntPtr handle)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   bounds:
        //     To be added.
        //
        //   annotationType:
        //     To be added.
        //
        //   properties:
        //     To be added.
        //     This parameter can be null.
        //
        // Remarks:
        //     To be added.
        [NullableContext(1)]
        [Export("initWithBounds:forType:withProperties:")]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [DesignatedInitializer]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public PdfAnnotation(CGRect bounds, NSString annotationType, NSDictionary? properties)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   bounds:
        //     To be added.
        //
        //   annotationType:
        //     To be added.
        //
        //   properties:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public PdfAnnotation(CGRect bounds, PdfAnnotationKey annotationType, NSDictionary? properties)
        {
            throw null;
        }

        //
        // Summary:
        //     Developers should not use this deprecated constructor. Developers should use
        //     '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.
        //
        // Parameters:
        //   bounds:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("initWithBounds:")]
        [Deprecated(PlatformName.iOS, 11, 0, PlatformArchitecture.None, "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.")]
        [Deprecated(PlatformName.MacOSX, 10, 12, PlatformArchitecture.None, "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.")]
        [Unavailable(PlatformName.MacCatalyst, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public PdfAnnotation(CGRect bounds)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   path:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("addBezierPath:")]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void AddBezierPath(UIBezierPath path)
        {
            throw null;
        }

        //
        // Summary:
        //     Performs a copy of the underlying Objective-C object.
        //
        // Parameters:
        //   zone:
        //     Zone to use to allocate this object, or null to use the default zone.
        //
        // Returns:
        //     This method performs a copy of the underlying Objective-C object state and returns
        //     a new instance of it. It does not actually try to replicate any managed state.
        //
        // Remarks:
        //     Implementation of the INSCopyable interface.
        [Export("copyWithZone:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [return: Release]
        public virtual NSObject Copy(NSZone? zone)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   box:
        //     To be added.
        //
        //   context:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("drawWithBox:inContext:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void Draw(PdfDisplayBox box, CGContext context)
        {
            throw null;
        }

        //
        // Summary:
        //     Encodes the state of the object on the provided encoder
        //
        // Parameters:
        //   encoder:
        //     The encoder object where the state of the object will be stored
        //
        // Remarks:
        //     This method is part of the Foundation.NSCoding protocol and is used by applications
        //     to preserve the state of the object into an archive.
        //     Users will typically create a Foundation.NSKeyedArchiver and then invoke the
        //     Foundation.NSKeyedArchiver.ArchiveRootObjectToFile(Foundation.NSObject,System.String)
        //     which will call into this method
        //     If developers want to allow their object to be archived, they should override
        //     this method and store their state in using the provided encoder parameter. In
        //     addition, developers should also implement a constructor that takes an NSCoder
        //     argument and is exported with [Export ("initWithCoder:")]
        //     public void override EncodeTo (NSCoder coder){
        //       coder.Encode (1, key: "version");
        //       coder.Encode (userName, key: "userName");
        //       coder.Encode (hostName, key: "hostName");
        [Export("encodeWithCoder:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void EncodeTo(NSCoder encoder)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   fromName:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("lineStyleFromName:")]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public static PdfLineStyle GetLineStyle(string fromName)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   style:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("nameForLineStyle:")]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public static string GetName(PdfLineStyle style)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   path:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("removeBezierPath:")]
        [Introduced(PlatformName.MacOSX, 10, 13, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void RemoveBezierPath(UIBezierPath path)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   key:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("removeValueForAnnotationKey:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        protected virtual void RemoveValue(NSString key)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   key:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public void RemoveValue(PdfAnnotationKey key)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   boolean:
        //     To be added.
        //
        //   key:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("setBoolean:forAnnotationKey:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        protected virtual bool SetValue(bool boolean, NSString key)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   boolean:
        //     To be added.
        //
        //   key:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public bool SetValue(bool boolean, PdfAnnotationKey key)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   rect:
        //     To be added.
        //
        //   key:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Export("setRect:forAnnotationKey:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        protected virtual bool SetValue(CGRect rect, NSString key)
        {
            throw null;
        }

        //
        // Summary:
        //     To be added.
        //
        // Parameters:
        //   rect:
        //     To be added.
        //
        //   key:
        //     To be added.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public bool SetValue(CGRect rect, PdfAnnotationKey key)
        {
            throw null;
        }

        [Export("valueForAnnotationKey:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        internal virtual IntPtr _GetValue(NSString key)
        {
            throw null;
        }

        [Export("setValue:forAnnotationKey:")]
        [Introduced(PlatformName.MacOSX, 10, 12, PlatformArchitecture.All, null)]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        internal virtual bool _SetValue(IntPtr value, NSString key)
        {
            throw null;
        }
    }
}
#if false // Decompilation log
'6' items in cache
------------------
Resolve: 'mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Found single assembly: 'mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Load from: 'C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\Xamarin.iOS\v1.0\mscorlib.dll'
------------------
Resolve: 'System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Found single assembly: 'System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Load from: 'C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\Xamarin.iOS\v1.0\System.dll'
------------------
Resolve: 'System.Xml, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Found single assembly: 'System.Xml, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'
Load from: 'C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\Xamarin.iOS\v1.0\System.Xml.dll'
------------------
Resolve: 'System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Could not find by name: 'System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
------------------
Resolve: 'System.Drawing.Common, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'System.Drawing.Common, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: 'C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\Xamarin.iOS\v1.0\Facades\System.Drawing.Common.dll'
#endif
