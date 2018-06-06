using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AMEC.WH.Converter
{
    public class FileSize2String : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is int))
                return "0 KB";

            int size = (int)value;

            return size < 1000 ? string.Format("{0} B", size) : string.Format("{0} KB", Math.Round(size / 1024.0, 2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
