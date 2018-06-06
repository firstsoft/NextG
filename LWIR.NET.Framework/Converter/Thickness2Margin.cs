using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LWIR.NET.Framework.Converter
{
    /// <summary>
    /// 边界宽度缩小1个像素
    /// </summary>
    internal class Thickness2Margin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new Thickness(0);

            if (!(value is Thickness))
                return new Thickness(0);

            Thickness oldThick = (Thickness)value;
            Thickness newThick = new Thickness(0);

            newThick.Left = oldThick.Left > 2 ? oldThick.Left - 2 : oldThick.Left;
            newThick.Right = oldThick.Right > 2 ? oldThick.Right - 2 : oldThick.Right;
            newThick.Top = oldThick.Top > 2 ? oldThick.Top - 2 : oldThick.Top;
            newThick.Bottom = oldThick.Bottom > 2 ? oldThick.Bottom - 2 : oldThick.Bottom;

            return newThick;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
