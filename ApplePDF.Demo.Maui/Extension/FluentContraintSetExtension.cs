using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace ApplePDF.Demo.Maui.Extension
{
    internal static class FluentContraintSetExtension
    {
        public static FluentConstraintSet.Element L2L(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.LeftToLeft(secondView, margin);
        }
        public static FluentConstraintSet.Element L2R(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.LeftToRight(secondView, margin);
        }
        public static FluentConstraintSet.Element R2L(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.RightToLeft(secondView, margin);
        }
        public static FluentConstraintSet.Element R2R(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.RightToRight(secondView, margin);
        }
        public static FluentConstraintSet.Element T2T(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.TopToTop(secondView, margin);
        }
        public static FluentConstraintSet.Element T2B(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.TopToBottom(secondView, margin);
        }
        public static FluentConstraintSet.Element B2B(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.BottomToBottom(secondView, margin);
        }
        public static FluentConstraintSet.Element B2T(this FluentConstraintSet.Element element, View secondView = null, int margin = 0)
        {
            return element.BottomToTop(secondView, margin);
        }
        /// <summary>
        /// 清除边的约束
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static FluentConstraintSet.Element ClearEdges(this FluentConstraintSet.Element element)
        {
            element.Clear(ConstrainedEdge.Center);
            return element.Clear(ConstrainedEdge.Baseline);
        }
    }
}
