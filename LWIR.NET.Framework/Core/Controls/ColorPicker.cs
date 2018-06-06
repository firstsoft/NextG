using LWIR.NET.Entity;
using LWIR.NET.Framework.Entity;
using LWIR.NET.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LWIR.NET.Framework.Core.Controls
{
    public class ColorPicker : Control
    {
        private Button cancelBtn = null;
        private Button okBtn = null;
        private Slider slider1 = null;
        private Slider slider2 = null;
        private Slider slider3 = null;
        private Slider slider4 = null;
        private ListBox lisBox = null;
        private Border customCanvas = null;
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
            EventManager.RegisterClassHandler(typeof(ColorPicker), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(ColorPicker), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            cancelBtn = GetTemplateChild("PART_CANCEL") as Button;
            okBtn = GetTemplateChild("PART_OK") as Button;
            slider1 = GetTemplateChild("PART_SLIDER1") as Slider;
            slider2 = GetTemplateChild("PART_SLIDER2") as Slider;
            slider3 = GetTemplateChild("PART_SLIDER3") as Slider;
            slider4 = GetTemplateChild("PART_SLIDER4") as Slider;
            lisBox = GetTemplateChild("PART_LISTBOX") as ListBox;
            customCanvas = GetTemplateChild("PART_CUSTOMCOLOR") as Border;

            this.AllBrushes = SystemParams.AllBrushes;

            if (cancelBtn != null)
            {
                cancelBtn.Click += cancelBtn_Click;
            }

            if (okBtn != null)
            {
                okBtn.Click += okBtn_Click;
            }

            if (lisBox != null)
            {
                lisBox.SelectionChanged += lisBox_SelectionChanged;
            }

            slider1.ValueChanged += slider_ValueChanged;
            slider2.ValueChanged += slider_ValueChanged;
            slider3.ValueChanged += slider_ValueChanged;
            slider4.ValueChanged += slider_ValueChanged;

            InnerBrush = SelectedBrush;
        }

        #region update UI
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte A1 = (byte)slider1.Value;
            byte R1 = (byte)slider2.Value;
            byte G1 = (byte)slider3.Value;
            byte B1 = (byte)slider4.Value;

            SolidColorBrush InnerBrush1 = new SolidColorBrush(Color.FromArgb(A1, R1, G1, B1));
            InnerBrush1.Freeze();
            InnerBrush = InnerBrush1;
        }

        void lisBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox1 = sender as ListBox;
            BrushInfo curBrush = listBox1.SelectedItem as BrushInfo;

            if (curBrush == null)
            {
                return;
            }

            InnerBrush = curBrush.CurBrush;
        }
        #endregion

        void okBtn_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
            SelectedBrush = InnerBrush;
        }

        void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            ColorPicker colorPicker = sender as ColorPicker;

            if (colorPicker == null)
                return;

            if (Mouse.Captured == colorPicker && e.OriginalSource == colorPicker)
            {
                colorPicker.IsDropDownOpen = false;
            }

            e.Handled = true;
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            ColorPicker colorPicker = sender as ColorPicker;
            MethodInfo method = typeof(MenuBase).GetMethod("IsDescendant", BindingFlags.NonPublic | BindingFlags.Static);
            bool IsDescendant = (bool)method.Invoke(null, new object[] { colorPicker, e.OriginalSource as DependencyObject });

            if (Mouse.Captured == colorPicker)
            {
                return;
            }

            if (e.OriginalSource == colorPicker)
            {
                if (Mouse.Captured == null || !IsDescendant)
                {
                    colorPicker.IsDropDownOpen = false;
                }
            }
            else
            {
                if (IsDescendant)
                {
                    if (colorPicker.IsDropDownOpen && Mouse.Captured == null || NativeMethods.GetCapture() == IntPtr.Zero)
                    {
                        Mouse.Capture(colorPicker, CaptureMode.SubTree);
                        e.Handled = true;
                    }
                }
                else
                {
                    colorPicker.IsDropDownOpen = false;
                }
            }
        }

        internal BrushInfo[] AllBrushes
        {
            get { return (BrushInfo[])GetValue(AllBasicColorsProperty); }
            set { SetValue(AllBasicColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BasicColors.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AllBasicColorsProperty =
            DependencyProperty.Register("AllBrushes", typeof(BrushInfo[]), typeof(ColorPicker), new PropertyMetadata(null));

        public SolidColorBrush SelectedBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedBrush", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.Black, InnerBrushChanged));

        internal SolidColorBrush InnerBrush
        {
            get { return (SolidColorBrush)GetValue(InnerBrushProperty); }
            set { SetValue(InnerBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TempColor.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InnerBrushProperty =
            DependencyProperty.Register("InnerBrush", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(Brushes.Black, InnerBrushChanged));

        private static void InnerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cp = d as ColorPicker;

            SolidColorBrush curBrush = (SolidColorBrush)e.NewValue;
            SolidColorBrush oldBrush = (SolidColorBrush)e.OldValue;

            if (curBrush.Color.A == oldBrush.Color.A &&
                curBrush.Color.R == oldBrush.Color.R &&
                curBrush.Color.G == oldBrush.Color.G &&
                curBrush.Color.B == oldBrush.Color.B)
                return;

            HlsColor hls = HSL.RgbToHls(curBrush.Color);

            cp.slider1.Value = curBrush.Color.A;
            cp.slider2.Value = curBrush.Color.R;
            cp.slider3.Value = curBrush.Color.G;
            cp.slider4.Value = curBrush.Color.B;

            double posX = hls.H * 280 / 360.0 - 5;
            double posY = (1 - hls.S) * 280 - 5;

            cp.customCanvas.Background = curBrush;
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorPicker), new PropertyMetadata(false, IsDropDownOpenChanged));

        private static void IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker colorPicker = d as ColorPicker;

            if (colorPicker == null)
                return;

            if (!(bool)e.NewValue)
            {
                Mouse.Capture(null);
            }
        }
    }
}
