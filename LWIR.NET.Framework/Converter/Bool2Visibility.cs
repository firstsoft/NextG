using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LWIR.NET.Framework.Converter
{
    /// <summary>
    /// Bool值转显示状态
    /// </summary>
    public class Bool2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Collapsed;

            bool tempValue = (bool)value;
            string condition = parameter == null ? "-" : parameter.ToString();

            if (condition.Equals("+"))
            {
                return tempValue ? Visibility.Visible : Visibility.Collapsed;
            }
            else//condition.Equals("-")
            {
                return tempValue ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
