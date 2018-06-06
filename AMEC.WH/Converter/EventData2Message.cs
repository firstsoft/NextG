using AMEC.Native;
using AMEC.WH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AMEC.WH.Converter
{
    public class EventData2Message : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is EventData))
                return String.Empty;

            EventData eventData = value as EventData;
            string eventFormat = EventModel.Instance.GetEventMessage(eventData.EventID, eventData.CurEventType, eventData.EventGroup);

            int percentIndex = eventFormat.IndexOf("%%");
            int dataIndex = 0;
            while (percentIndex > -1)
            {
                if (dataIndex >= 4) //handle event definition and parameter not match sititation
                    break;

                switch (eventData.CurDataType[dataIndex])
                {
                    case DataType.None:
                        eventFormat = eventFormat.Substring(0, percentIndex) + "Invaild Parameter" + eventFormat.Substring(percentIndex + 2);
                        break;
                    case DataType.Int:
                        eventFormat = eventFormat.Substring(0, percentIndex) + eventData.IntData[dataIndex].ToString() + eventFormat.Substring(percentIndex + 2);
                        break;
                    case DataType.IntHex:
                        eventFormat = eventFormat.Substring(0, percentIndex) + eventData.IntData[dataIndex].ToString("X") + eventFormat.Substring(percentIndex + 2);
                        break;
                    case DataType.Float:
                        eventFormat = eventFormat.Substring(0, percentIndex) + eventData.FloatData[dataIndex] + eventFormat.Substring(percentIndex + 2);
                        break;
                    case DataType.String:
                        break;
                    case DataType.Attribute:
                        break;
                    case DataType.Enum:
                        {
                            int tempValue = eventData.IntData[dataIndex];
                            int enumID = tempValue >> 16;
                            int enumVal = (tempValue << 16) >> 16;

                            string enumStr = string.Format("[enumId:{0},enumValue:{1}]", enumID, enumVal);
                            eventFormat = eventFormat.Substring(0, percentIndex) + enumStr + eventFormat.Substring(percentIndex + 2);
                            break;
                        }
                    default:
                        break;
                }

                dataIndex++;
                percentIndex = eventFormat.IndexOf("%%");
            }


            return eventFormat;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
