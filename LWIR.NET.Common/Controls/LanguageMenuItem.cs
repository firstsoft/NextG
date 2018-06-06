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

namespace LWIR.NET.Common.Controls
{
    public class LanguageMenuItem : RadioButton
    {
        static LanguageMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageMenuItem), new FrameworkPropertyMetadata(typeof(LanguageMenuItem)));
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(LanguageMenuItem), new PropertyMetadata(string.Empty));

        public BitmapSource Icon
        {
            get { return (BitmapSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(BitmapSource), typeof(LanguageMenuItem), new PropertyMetadata(null));
    }
}
