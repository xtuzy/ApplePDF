using System;
using System.Collections.Generic;
using System.Text;

namespace ApllePDF.Binding
{
    internal static class Setting
    {
#if __IOS__
        internal const string DllName = "__Internal";
#elif __ANDROID__
        internal const string DllName = "pdfium.so";
#else
        internal const string DllName = "pdfium";
#endif
    }
}
