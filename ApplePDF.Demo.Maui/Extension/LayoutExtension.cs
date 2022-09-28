using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.Demo.Maui.Extension
{
    internal static class LayoutExtension
    {
        public static void AddViews(this Layout layout, params View[] views)
        {
            foreach(View view in views)
                layout.Add(view);
        }
    }
}
