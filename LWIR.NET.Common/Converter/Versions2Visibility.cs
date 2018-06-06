using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LWIR.NET.Common.Converter
{
    /// <summary>
    /// 版本对比判断控件显示与否
    /// </summary>
    public class Versions2Visibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return Visibility.Collapsed;

            if (parameter == null)
                return Visibility.Collapsed;

            if (!(values[0] is string) || !(values[1] is string))
                return Visibility.Collapsed;

            if (parameter.ToString().Equals("+"))
            {
                return string.Compare(values[0].ToString(), values[1].ToString()) > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return string.Compare(values[0].ToString(), values[1].ToString()) > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
