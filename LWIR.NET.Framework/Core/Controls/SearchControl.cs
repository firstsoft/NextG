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
    public class SearchControl : TextBox
    {
        private Button searchBtn = null;
        static SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl), new FrameworkPropertyMetadata(typeof(SearchControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            searchBtn = GetTemplateChild("PART_SEARCHBTN") as Button;

            if (searchBtn != null)
            {
                searchBtn.Click += searchBtn_Click;
            }

            this.MouseRightButtonUp += TextBoxEx_MouseRightButtonUp;
        }

        /// <summary>
        /// 如果是有搜索内容，点击则删除，否则无反应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.Text = string.Empty;
            }
        }

        /// <summary>
        /// Text to display as watermark
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WatermarkText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(SearchControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The foreground of watermark
        /// </summary>
        public Brush WatermarkForeground
        {
            get { return (Brush)GetValue(WatermarkForegroundProperty); }
            set { SetValue(WatermarkForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WatermarkForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(SearchControl), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// The Opacity of Watermark
        /// </summary>
        public double WatermarkOpacity
        {
            get { return (double)GetValue(WatermarkOpacityProperty); }
            set { SetValue(WatermarkOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WatermarkOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkOpacityProperty =
            DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(SearchControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Enabled or disabled the contexmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBoxEx_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
