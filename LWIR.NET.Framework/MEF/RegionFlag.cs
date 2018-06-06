using Microsoft.Practices.ServiceLocation;
using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.MEF
{
    public class RegionFlag : DependencyObject
    {
        public static string GetRegionName(DependencyObject obj)
        {
            return (string)obj.GetValue(RegionNameProperty);
        }

        public static void SetRegionName(DependencyObject obj, string value)
        {
            obj.SetValue(RegionNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for RegionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionFlag), new PropertyMetadata(string.Empty, RegionNameChanged));

        private static void RegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // add region to regionManager
            if (ServiceLocator.Current == null)
                return;

            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

            if (regionManager == null)
                return;

            string regionName = (string)e.NewValue;

            if (string.IsNullOrWhiteSpace(regionName))
                throw new InvalidOperationException("RegionName should not be empty");

            regionManager.AddRegion(regionName, d);
        }

    }
}
