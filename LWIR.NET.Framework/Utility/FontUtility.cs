using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LWIR.NET.Framework.Utility
{
    internal class FontUtility
    {
        /// <summary>
        /// Get default font family
        /// </summary>
        /// <param name="fonts"></param>
        /// <returns></returns>
        public static FontFamily GetDefaultFontFamily(ICollection<FontFamily> fonts)
        {
            FontFamily curFonts = fonts.FirstOrDefault(f => f.Source.Equals("Arial"));

            if (curFonts == null)
                curFonts = fonts.FirstOrDefault(f => f.Source.Equals("Times New Roman"));

            if (curFonts == null)
                curFonts = fonts.FirstOrDefault();

            return curFonts;
        }
    }
}
