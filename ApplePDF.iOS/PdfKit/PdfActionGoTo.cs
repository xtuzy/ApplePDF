namespace ApplePDF.PdfKit
{
    /// <summary>
    /// PDFActionGoTo, a subclass of , defines methods for getting and setting the destination of a go-to action.PDFAction
    /// <br/>
    /// A object represents the action of going to a specific location within the PDF document.PDFActionGoTo
    /// </summary>
    internal class PdfActionGoTo : PdfAction
    {
        public PdfActionGoTo(PdfDocument document, PdfOutline outline)
        {
            this.Type = "PdfActionGoTo";
            this.document = document;
            Action = outline.Outline.Action;
        }

        PdfDestination destination;
        /// <summary>
        /// Gets or sets the destination of the go-to PDF action.
        /// </summary>
        public PdfDestination Destination
        {
            get
            {
                return new PdfDestination();
            }
        }
    }
}
