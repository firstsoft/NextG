using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LWIR.NET.Entity
{
    /// <summary>
    /// List of the points for curve
    /// </summary>
    public class CurvesData : INotifyPropertyChanged
    {
        private SolidColorBrush curveBrush = Brushes.Orange;
        /// <summary>
        /// Brush for current curve
        /// </summary>
        public SolidColorBrush CurveBrush
        {
            get { return curveBrush; }
            set { curveBrush = value; RaisePropertyChanged("CurveBrush"); }
        }

        private List<DataPoint> dataPoints = new List<DataPoint>();
        /// <summary>
        /// Data collection for current curve
        /// </summary>
        public List<DataPoint> DataPoints
        {
            get { return dataPoints; }
            set { dataPoints = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
