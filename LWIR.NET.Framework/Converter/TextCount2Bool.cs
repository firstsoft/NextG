using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LWIR.NET.Framework.Converter
{
    /// <summary>
    /// true if count>0
    /// </summary>
    public class TextCount2Bool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
            {
                return false;
            }
            var tempText = (string)value;
            return !string.IsNullOrEmpty(tempText);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
