using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LWIR.NET.Framework.Core.Controls
{
    /// <summary>
    /// The limit mode for input
    /// </summary>
    public enum InputLimitMode
    {
        /// <summary>
        /// Only number
        /// </summary>
        Numberic,

        /// <summary>
        /// Only letters, not case sensitive
        /// </summary>
        Letters,

        /// <summary>
        /// number and letters
        /// </summary>
        NumbericAndLetters,

        /// <summary>
        /// Custom regex for limit
        /// </summary>
        Custom,

        /// <summary>
        /// without limit
        /// </summary>
        None,
    }

    /// <summary>
    /// A extended control for Textbox
    /// </summary>
    public class TextBoxEx : TextBox
    {
        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseRightButtonUp += TextBoxEx_MouseRightButtonUp;
            this.PreviewTextInput += TextBoxEx_PreviewTextInput;
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(DataPasting_Callback));
        }

        /// <summary>
        /// Input limit mode
        /// </summary>
        public InputLimitMode InputLimitMode
        {
            get { return (InputLimitMode)GetValue(InputLimitModeProperty); }
            set { SetValue(InputLimitModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputLimitMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputLimitModeProperty =
            DependencyProperty.Register("InputLimitMode", typeof(InputLimitMode), typeof(TextBoxEx), new PropertyMetadata(InputLimitMode.None, new PropertyChangedCallback(InputLimitModeChanged)));

        private static void InputLimitModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxEx textBoxEx = d as TextBoxEx;

            InputMethod.SetIsInputMethodEnabled(textBoxEx, (InputLimitMode)e.NewValue == InputLimitMode.None);
        }

        /// <summary>
        /// Custom define regex for input limit.
        /// </summary>
        public string InputLimitRegex
        {
            get { return (string)GetValue(InputLimitRegexProperty); }
            set { SetValue(InputLimitRegexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputLimitRegex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputLimitRegexProperty =
            DependencyProperty.Register("InputLimitRegex", typeof(string), typeof(TextBoxEx), new PropertyMetadata(string.Empty));


        /// <summary>
        /// A switch for enable or disable the input method
        /// </summary>
        public new bool IsInputMethodEnabled
        {
            get { return (bool)GetValue(IsInputMethodEnabledProperty); }
            set { SetValue(IsInputMethodEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInputMethodLocked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInputMethodEnabledProperty =
            DependencyProperty.Register("IsInputMethodEnabled", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(true, new PropertyChangedCallback(IsInputMethodEnabledChanged)));

        private static void IsInputMethodEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxEx textBoxEx = d as TextBoxEx;

            InputMethod.SetIsInputMethodEnabled(textBoxEx, (bool)e.NewValue);
        }

        /// <summary>
        /// Watermark enabled or disabled
        /// </summary>
        public bool IsWatermarkEnabled
        {
            get { return (bool)GetValue(IsWatermarkEnabledProperty); }
            set { SetValue(IsWatermarkEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWatermarkEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWatermarkEnabledProperty =
            DependencyProperty.Register("IsWatermarkEnabled", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(false));

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
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(TextBoxEx), new PropertyMetadata(string.Empty));

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
            DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(TextBoxEx), new PropertyMetadata(Brushes.Black));

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
            DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(TextBoxEx), new PropertyMetadata(1.0));

        /// <summary>
        /// Enabled or disabled the contextmenu
        /// </summary>
        public bool IsContextMenuEnabled
        {
            get { return (bool)GetValue(IsContextMenuEnabledProperty); }
            set { SetValue(IsContextMenuEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextMenuEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContextMenuEnabledProperty =
            DependencyProperty.Register("IsContextMenuEnabled", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(true));

        /// <summary>
        /// Enabled or disabled the contexmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBoxEx_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsContextMenuEnabled)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// For pasting filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataPasting_Callback(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// For input filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBoxEx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Judge input limit is valid or not
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsTextAllowed(string text)
        {
            string curRegexString = string.Empty;
            switch (this.InputLimitMode)
            {
                case InputLimitMode.Numberic:
                    curRegexString = @"^[1-9]\d*$";
                    break;
                case InputLimitMode.Letters:
                    curRegexString = @"^[A-Za-z]+$";
                    break;
                case InputLimitMode.NumbericAndLetters:
                    curRegexString = @"^[A-Za-z0-9]+$";
                    break;
                case InputLimitMode.Custom:
                    curRegexString = this.InputLimitRegex;
                    break;
                default:// InputLimitMode.None:
                    return true;
            }

            try
            {
                Regex rgx = new Regex(curRegexString);
                return rgx.IsMatch(text);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
