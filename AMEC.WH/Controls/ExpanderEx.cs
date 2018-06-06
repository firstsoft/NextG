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

namespace AMEC.WH.Controls
{
    public class ExpanderEx : Control
    {
        static ExpanderEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderEx), new FrameworkPropertyMetadata(typeof(ExpanderEx)));
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ExpanderEx), new PropertyMetadata(null));


        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ExpanderEx), new PropertyMetadata(null));



        public HorizontalAlignment HeaderContentHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderContentHorizontalAlignmentProperty); }
            set { SetValue(HeaderContentHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderContentHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderContentHorizontalAlignmentProperty =
            DependencyProperty.Register("HeaderContentHorizontalAlignment", typeof(HorizontalAlignment), typeof(ExpanderEx), new PropertyMetadata(HorizontalAlignment.Left));



        public VerticalAlignment HeaderContentVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(HeaderContentVerticalAlignmentProperty); }
            set { SetValue(HeaderContentVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderContentVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderContentVerticalAlignmentProperty =
            DependencyProperty.Register("HeaderContentVerticalAlignment", typeof(VerticalAlignment), typeof(ExpanderEx), new PropertyMetadata(VerticalAlignment.Center));



        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExpanderEx), new PropertyMetadata(false));


        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ExpanderEx), new PropertyMetadata(Brushes.Black));


    }
}
