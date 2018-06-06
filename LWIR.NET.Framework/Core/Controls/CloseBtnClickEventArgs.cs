using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.Core.Controls
{
    public class CloseBtnClickEventArgs: RoutedEventArgs
    {
        public CloseBtnClickEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }
    }
}
