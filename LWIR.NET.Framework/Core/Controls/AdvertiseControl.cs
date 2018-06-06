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
    public class AdvertiseControl : Control
    {
        private Button closeBtn = null;
        static AdvertiseControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvertiseControl), new FrameworkPropertyMetadata(typeof(AdvertiseControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            closeBtn = GetTemplateChild("PART_CLOSEBTN") as Button;

            if (closeBtn != null)
            {
                closeBtn.Click += closeBtn_Click;
            }
        }

        /// <summary>
        /// 关闭按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = false;
        }

        /// <summary>
        /// 是否关闭广告
        /// </summary>
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsClosed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register("IsClosed", typeof(bool), typeof(AdvertiseControl), new PropertyMetadata(true, IsClosedChanged));

        private static void IsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvertiseControl ctrl = d as AdvertiseControl;

            ctrl.Visibility = ctrl.IsClosed ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 广告内容
        /// </summary>
        public string Advertisement
        {
            get { return (string)GetValue(AdvertisementProperty); }
            set { SetValue(AdvertisementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Advertisement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdvertisementProperty =
            DependencyProperty.Register("Advertisement", typeof(string), typeof(AdvertiseControl), new PropertyMetadata(string.Empty));
    }
}
