using System;
using System.Drawing;

namespace Pdf.Net.PdfKit
{
    public struct PdfCharacter : IEquatable<PdfCharacter>
    {
        private const double Tolerance = 0.001;

        public char Char { get; }

        public RectangleF Box { get; }

        public float Angle { get; }

        public double FontSize { get; }

        public PdfCharacter(char character, RectangleF box, float angle, double fontSize)
        {
            Char = character;
            Box = box;
            Angle = angle;
            FontSize = fontSize;
        }

        public static bool operator ==(PdfCharacter obj1, PdfCharacter obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(PdfCharacter obj1, PdfCharacter obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(PdfCharacter other)
        {
            return Char == other.Char
                   && Box.Equals(other.Box)
                   && Math.Abs(Angle - other.Angle) < Tolerance
                   && Math.Abs(FontSize - other.FontSize) < Tolerance;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PdfCharacter))
            {
                return false;
            }

            var character = (PdfCharacter)obj;

            return Equals(character);
        }

        public override int GetHashCode()
        {
            var hashCode = 13;
            hashCode = hashCode * 7 + Char.GetHashCode();
            hashCode = hashCode * 7 + Box.GetHashCode();
            return hashCode;
        }
    }
}