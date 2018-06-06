using AMEC.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AMEC.WH.Converter
{
    public class Wh2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is WaferHistoryInfo))
                return Visibility.Collapsed;

            WaferHistoryInfo info = value as WaferHistoryInfo;

            if (info == null)
                return Visibility.Collapsed;

            if (info.WaferDataBlocks == null)
                return Visibility.Collapsed;

            if (info.WaferDataBlocks.Any(w => w.SubWaferDataBlocks != null && w.SubWaferDataBlocks.Any(s => File.Exists(s.CurRecipeInfo.DataCollectionFullName))))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
