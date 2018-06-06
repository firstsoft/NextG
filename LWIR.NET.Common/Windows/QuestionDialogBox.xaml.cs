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
    /// Interaction logic for QuestionDialogBox.xaml
    /// </summary>
    public partial class QuestionDialogBox : BaseWnd
    {
        public static bool ShowDialog(string title, string msgText, string yesBtnText = "Yes", string noBtnText = "No", string cancelBtnText = "Cancel")
        {
            QuestionDialogBox msgBox = new QuestionDialogBox();
            msgBox.Title = title;
            msgBox.Text = msgText;
            msgBox.YesBtnText = yesBtnText;
            msgBox.NoBtnText = noBtnText;
            msgBox.CancelBtnText = cancelBtnText;
            msgBox.Owner = Application.Current.MainWindow;
            return msgBox.ShowDialog().Value;
        }

        public QuestionDialogBox()
        {
            InitializeComponent();
        }

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
            DependencyProperty.Register("Text", typeof(string), typeof(QuestionDialogBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 是按钮上的文字显示
        /// </summary>
        public string YesBtnText
        {
            get { return (string)GetValue(YesBtnTextProperty); }
            set { SetValue(YesBtnTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OKBtnText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesBtnTextProperty =
            DependencyProperty.Register("YesBtnText", typeof(string), typeof(QuestionDialogBox), new PropertyMetadata("Yes"));

        /// <summary>
        /// 否按钮上的文字显示
        /// </summary>
        public string NoBtnText
        {
            get { return (string)GetValue(NoBtnTextProperty); }
            set { SetValue(NoBtnTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OKBtnText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoBtnTextProperty =
            DependencyProperty.Register("NoBtnText", typeof(string), typeof(QuestionDialogBox), new PropertyMetadata("No"));

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
            DependencyProperty.Register("CancelBtnText", typeof(string), typeof(QuestionDialogBox), new PropertyMetadata("Cancel"));

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
