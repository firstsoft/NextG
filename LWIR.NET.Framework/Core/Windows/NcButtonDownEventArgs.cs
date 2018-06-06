using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.Core.Windows
{
    public class NcButtonDownEventArgs : RoutedEventArgs
    {
        public NcButtonDownEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }
    }

    public class AboutMenuEventArgs : RoutedEventArgs
    {
        public AboutMenuEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }
    }
}
