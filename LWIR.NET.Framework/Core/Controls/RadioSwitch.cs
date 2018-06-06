using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LWIR.NET.Framework.Core.Controls
{
    public class RadioSwitch : Control
    {
        static RadioSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioSwitch), new FrameworkPropertyMetadata(typeof(RadioSwitch)));
        }
    }
}
