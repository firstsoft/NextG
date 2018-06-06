using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LWIR.NET.Framework.Core.Controls
{
    public class TextBlockEx : TextBlock
    {
        /// <summary>
        /// Binding key for dynamic resource
        /// </summary>
        public string BindingKey
        {
            get { return (string)GetValue(BindingKeyProperty); }
            set { SetValue(BindingKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindingKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingKeyProperty =
            DependencyProperty.Register("BindingKey", typeof(string), typeof(TextBlockEx), new PropertyMetadata(null, new PropertyChangedCallback(BindingKeyChanged)));

        private static void BindingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlockEx t = d as TextBlockEx;
            if (!string.IsNullOrEmpty(t.BindingKey))
            {
                t.SetResourceReference(TextBlockEx.TextProperty, t.BindingKey);
            }
        }
    }
}
