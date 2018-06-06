using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// Extend for brush
    /// </summary>
    public static class BrushExtend
    {
        /// <summary>
        /// Get color from brush, return float array [r,g,b,a]
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static float[] GetRGBAf(this Brush brush)
        {
            if (brush == null)
            {
                return new float[4] { 0, 0, 0, 0 };
            }

            float[] curColor = new float[4];

            byte a = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).A;
            byte g = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).G;
            byte r = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).R;
            byte b = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).B;

            curColor[0] = r / (float)byte.MaxValue;
            curColor[1] = g / (float)byte.MaxValue;
            curColor[2] = b / (float)byte.MaxValue;
            curColor[3] = a / (float)byte.MaxValue;

            return curColor;
        }

        /// <summary>
        /// Get color from brush, return float array [r,g,b,a]
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static byte[] GetRGBAb(this Brush brush)
        {
            if (brush == null)
            {
                return new byte[4] { 0, 0, 0, 0 };
            }

            byte[] curColor = new byte[4];

            byte a = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).A;
            byte g = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).G;
            byte r = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).R;
            byte b = ((Color)brush.GetValue(SolidColorBrush.ColorProperty)).B;

            curColor[0] = r;
            curColor[1] = g;
            curColor[2] = b;
            curColor[3] = a;

            return curColor;
        }
    }
}
