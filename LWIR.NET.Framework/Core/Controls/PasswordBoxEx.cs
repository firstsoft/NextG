using LWIR.NET.Framework.Utility;
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
    public class PasswordBoxEx : Control
    {
        private PasswordBox pwdBox = null;

        static PasswordBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordBoxEx), new FrameworkPropertyMetadata(typeof(PasswordBoxEx)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            pwdBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;

            if (pwdBox != null)
            {
                pwdBox.PasswordChanged += pwdBox_PasswordChanged;

                // resolve the bug in XP or some OS which has no the default font
                var fonts = Fonts.SystemFontFamilies;

                if (fonts.Count == 0)
                    return;

                FontFamily curFonts = null;

                if (pwdBox.FontFamily == null || !fonts.Any(f => f.Source.Equals(pwdBox.FontFamily.Source)))
                {
                    curFonts = FontUtility.GetDefaultFontFamily(fonts);
                }
                else
                {
                    curFonts = pwdBox.FontFamily;
                }

                pwdBox.FontFamily = curFonts;
            }
        }

        /// <summary>
        /// 密码框中的密码修改导致绑定值属性修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!Password.Equals(pwdBox.Password))
            {
                Binding binding = BindingOperations.GetBinding(this, PasswordBoxEx.PasswordProperty);

                switch (binding.Mode)
                {
                    case BindingMode.OneWay:
                        break;
                    case BindingMode.TwoWay:
                        Password = pwdBox.Password;
                        break;
                    case BindingMode.OneWayToSource:
                        Password = pwdBox.Password;
                        break;
                }
            }
        }

        /// <summary>
        /// 密码绑定属性
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxEx), new PropertyMetadata(string.Empty, new PropertyChangedCallback(PasswordChanged)));

        /// <summary>
        /// 密码根据绑定属性的模式进行值的修改
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void PasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBoxEx tempPwdBox = d as PasswordBoxEx;

            if (!tempPwdBox.Password.Equals(tempPwdBox.pwdBox.Password))
            {
                Binding binding = BindingOperations.GetBinding(tempPwdBox, PasswordBoxEx.PasswordProperty);

                switch (binding.Mode)
                {
                    case BindingMode.OneWay:
                    case BindingMode.TwoWay:
                        tempPwdBox.pwdBox.Password = tempPwdBox.Password;
                        break;
                }
            }
        }

        /// <summary>
        /// 密码显示的字符
        /// </summary>
        public Char PasswordChar
        {
            get { return (Char)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PasswordChar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register("PasswordChar", typeof(Char), typeof(PasswordBoxEx), new PropertyMetadata('*'));

        /// <summary>
        /// Tab index
        /// </summary>
        public new int TabIndex
        {
            get { return (int)GetValue(TabIndexProperty); }
            set { SetValue(TabIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabIndex.  This enables animation, styling, binding, etc...
        public static readonly new DependencyProperty TabIndexProperty =
            DependencyProperty.Register("TabIndex", typeof(int), typeof(PasswordBoxEx), new PropertyMetadata(0));
    }
}
