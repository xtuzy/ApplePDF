using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Net.Test.Extensions
{
    internal static class ColorExtesnion
    {
        public static bool IsEqual(this Color except, Color actual)
        {
            return except.A == actual.A &&
                except.R == actual.R &&
                        except.G == actual.G &&
                        except.B == actual.B;
        }
    }
}
