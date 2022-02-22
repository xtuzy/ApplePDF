# ApplePDF
## What is it?
ApplePDF is a .net library for read and operate PDF file base on pdfium. Its api like Xamarin.iOS.PdfKit, so i name it ApplePDF.

It support:
- Linux
- Windows
- Mac
- Android

It not support iOS, because iOS have Pdfkit, it's well, if you want use pdfium at iOS, you can find compiled pdfium native library by them:
- [bblanchon's pdfium-binaries](https://github.com/bblanchon/pdfium-binaries)
- [paulo-coutinho's pdfium-lib](https://github.com/paulo-coutinho/pdfium-lib)

This library many code reference to [docnet](https://github.com/GowenGit/docnet).(I'm a newcomer at .net, so this library will have some bug, i advice you create your test for this library, or just reference how to use pdfium)

This library use .NET Standard P/Invoke bindings for PDFium form 
[DJGosnell's PDFiumCore](https://github.com/Dtronix/PDFiumCore)

Very thanks them!

## Some Resource
- [pdfium source](https://github.com/QPDFium/pdfium/tree/master/fpdfsdk)
    You can know how to use pdfium form test files.
- [QuestPDF](https://github.com/QuestPDF/QuestPDF)
    You can use it draw pdf.
