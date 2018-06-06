using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LWIR.NET.Framework.Converter
{
    /// <summary>
    /// 水印的显示隐藏转换
    /// </summary>
    internal class TextCount2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
            {
                return Visibility.Collapsed;
            }
            var tempText = (string)value;
            return string.IsNullOrEmpty(tempText) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
