using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LWIR.NET.Framework.Core.Controls
{
    public class LuminosityBar : Slider
    {
        static LuminosityBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LuminosityBar), new FrameworkPropertyMetadata(typeof(LuminosityBar)));
        }
    }
}
