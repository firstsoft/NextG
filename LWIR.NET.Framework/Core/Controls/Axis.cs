using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LWIR.NET.Framework.Core.Controls
{
    public class Axis : Canvas
    {
        public Axis()
        {
            this.SnapsToDevicePixels = true;
        }

        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set { SetValue(YLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YLabelProperty =
            DependencyProperty.Register("YLabel", typeof(string), typeof(Axis), new PropertyMetadata("YLabel"));

        


        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
        }
    }
}
