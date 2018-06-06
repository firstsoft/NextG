using LWIR.NET.Framework.Core.Windows;
using LWIR.NET.Framework.Enum;
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
    /// Interaction logic for MessageBoxEx.xaml
    /// </summary>
    public partial class MessageBoxEx : BaseWnd
    {
        public static bool ShowDialog(DialogType dialogType, string title, string msgText, string okBtnText = "OK", string cancelBtnText = "Cancel")
        {
            MessageBoxEx msgBox = new MessageBoxEx();
            msgBox.DialogType = dialogType;
            msgBox.Title = title;
            msgBox.Text = msgText;
            msgBox.OKBtnText = okBtnText;
            msgBox.CancelBtnText = cancelBtnText;
            msgBox.Owner = Application.Current.MainWindow;
            return msgBox.ShowDialog().Value;
        }

        public MessageBoxEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 对话框类型
        /// </summary>
        public DialogType DialogType
        {
            get { return (DialogType)GetValue(DialogTypeProperty); }
            set { SetValue(DialogTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogTypeProperty =
            DependencyProperty.Register("DialogType", typeof(DialogType), typeof(MessageBoxEx), new PropertyMetadata(DialogType.Hint));

        /// <summary>
        /// 显示的文字
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MessageBoxEx), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 确定按钮上的文字显示
        /// </summary>
        public string OKBtnText
        {
            get { return (string)GetValue(OKBtnTextProperty); }
            set { SetValue(OKBtnTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OKBtnText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OKBtnTextProperty =
            DependencyProperty.Register("OKBtnText", typeof(string), typeof(MessageBoxEx), new PropertyMetadata("OK"));

        /// <summary>
        /// 取消按钮上的文字显示
        /// </summary>
        public string CancelBtnText
        {
            get { return (string)GetValue(CancelBtnTextProperty); }
            set { SetValue(CancelBtnTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelBtnText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelBtnTextProperty =
            DependencyProperty.Register("CancelBtnText", typeof(string), typeof(MessageBoxEx), new PropertyMetadata("Cancel"));

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
