namespace ApplePDF.PdfKit
{
    /// <summary>
    /// An action that is performed when, for example, a PDF annotation is activated or an outline item is clicked.
    /// <br/>
    /// A PDFAction object represents an action associated with a PDF element, such as an annotation or a link, that the viewer application can perform. See the Adobe PDF Specification for more about actions and action types.
    /// PDFAction is an abstract superclass of the following concrete classes:
    /// PDFActionGoTo
    /// PDFActionNamed
    /// PDFActionRemoteGoTo
    /// PDFActionResetForm
    /// PDFActionURL
    /// </summary>
    public abstract class PdfAction : IPdfAction
    {
        public string Type { get; set; }

        public PlatformPdfAction Action;
        protected PdfDocument document;

        public void Dispose()
        {
            document = null;
            Action = null;
        }
    }
}
