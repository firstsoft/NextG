using AMEC.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace AMEC.WH.Converter
{
    class EventType2Brush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is EventType))
                return Brushes.White;

            EventType eventType = (EventType)value;

            if (eventType == EventType.WARNING)
            {
                return Brushes.Orange;
            }
            else if (eventType == EventType.ALARM)
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
