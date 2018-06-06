using LWIR.NET.Framework.Core.Windows;
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
using System.Windows.Shapes;

namespace LWIR.NET.Common.Windows
{
    /// <summary>
    /// Interaction logic for UpgradeWindow.xaml
    /// </summary>
    public partial class UpgradeWindow : BaseWnd
    {
        public UpgradeWindow()
        {
            InitializeComponent();
        }

        public string NewVersion
        {
            get { return (string)GetValue(NewVersionProperty); }
            set { SetValue(NewVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewVersionProperty =
            DependencyProperty.Register("NewVersion", typeof(string), typeof(UpgradeWindow), new PropertyMetadata("1.0.0.0"));

        public string OldVersion
        {
            get { return (string)GetValue(OldVersionProperty); }
            set { SetValue(OldVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OldVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OldVersionProperty =
            DependencyProperty.Register("OldVersion", typeof(string), typeof(UpgradeWindow), new PropertyMetadata("1.0.0.0"));

        /// <summary>
        /// 是否不显示通知提示
        /// </summary>
        public bool IsNotShowHint
        {
            get { return (bool)GetValue(IsNotShowHintProperty); }
            set { SetValue(IsNotShowHintProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNotShowHint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNotShowHintProperty =
            DependencyProperty.Register("IsNotShowHint", typeof(bool), typeof(UpgradeWindow), new PropertyMetadata(false));

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
