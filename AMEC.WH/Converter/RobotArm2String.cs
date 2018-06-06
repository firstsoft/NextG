using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AMEC.WH.Converter
{
    public class RobotArm2String : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is int))
                return String.Empty;

            int robotArm = (int)value;

            return string.Format("{0}({1})", robotArm == 0 ? "Lower Arm" : "Upper Arm", robotArm);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
