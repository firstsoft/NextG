using Microsoft.Windows.Shell;
using LWIR.NET.Framework.Entity;
using LWIR.NET.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LWIR.NET.Framework.Core.Windows
{
    public class BaseWnd : Window
    {
        private Button minBtn, maxBtn, closeBtn;
        private HwndSource curHwndSource = null;
        private HwndSourceHook curHwndSourceHook = null;
        private WindowChrome wndChrome = new WindowChrome();

        public static readonly RoutedEvent NcButtonDownEvent = EventManager.RegisterRoutedEvent("NcButtonDown", RoutingStrategy.Bubble, typeof(EventHandler<NcButtonDownEventArgs>), typeof(BaseWnd));
        //CLR事件包装  
        public event RoutedEventHandler NcButtonDown
        {
            add { this.AddHandler(NcButtonDownEvent, value); }
            remove { this.RemoveHandler(NcButtonDownEvent, value); }
        }

        public static readonly RoutedEvent AboutMenuEvent = EventManager.RegisterRoutedEvent("AboutMenuClick", RoutingStrategy.Bubble, typeof(EventHandler<AboutMenuEventArgs>), typeof(BaseWnd));
        //CLR事件包装  
        public event RoutedEventHandler AboutMenuClick
        {
            add { this.AddHandler(AboutMenuEvent, value); }
            remove { this.RemoveHandler(AboutMenuEvent, value); }
        }

        static BaseWnd()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseWnd), new FrameworkPropertyMetadata(typeof(BaseWnd)));
        }

        #region 属性
        /// <summary>
        /// 非客户区背景颜色
        /// </summary>
        public Brush NonClientBackground
        {
            get { return (Brush)GetValue(NonClientBackgroundProperty); }
            set { SetValue(NonClientBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonClientBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonClientBackgroundProperty =
            DependencyProperty.Register("NonClientBackground", typeof(Brush), typeof(BaseWnd), new PropertyMetadata(Brushes.Green));

        /// <summary>
        /// 边框宽度
        /// </summary>
        public Thickness BorderSize
        {
            get { return (Thickness)GetValue(BorderSizeProperty); }
        }

        // Using a DependencyProperty as the backing store for BorderSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderSizeProperty =
            DependencyProperty.Register("BorderSize", typeof(Thickness), typeof(BaseWnd), new PropertyMetadata(new Thickness(7.0)));

        /// <summary>
        /// 标题栏内容
        /// </summary>
        public object CaptionContent
        {
            get { return (object)GetValue(CaptionContentProperty); }
            set { SetValue(CaptionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaptionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionContentProperty =
            DependencyProperty.Register("CaptionContent", typeof(object), typeof(BaseWnd), new PropertyMetadata(null));

        /// <summary>
        /// 系统按钮的扩展内容
        /// </summary>
        public object SystemButtonsExtendContent
        {
            get { return (object)GetValue(SystemButtonsExtendContentProperty); }
            set { SetValue(SystemButtonsExtendContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SystemButtonsExtendContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SystemButtonsExtendContentProperty =
            DependencyProperty.Register("SystemButtonsExtendContent", typeof(object), typeof(BaseWnd), new PropertyMetadata(null));

        /// <summary>
        /// 标题栏高度
        /// </summary>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaptionHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(BaseWnd), new PropertyMetadata(30.0, CaptionHeightChanged));

        private static void CaptionHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseWnd baseWnd = d as BaseWnd;
            baseWnd.wndChrome.CaptionHeight = (double)e.NewValue;
        }

        /// <summary>
        /// 最小化按钮的显示状态
        /// </summary>
        public Visibility MinBtnVisibility
        {
            get { return (Visibility)GetValue(MinBtnVisibilityProperty); }
            set { SetValue(MinBtnVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinBtnVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinBtnVisibilityProperty =
            DependencyProperty.Register("MinBtnVisibility", typeof(Visibility), typeof(BaseWnd), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 最小化按钮的tooltip
        /// </summary>
        public string MinBtnTooltip
        {
            get { return (string)GetValue(MinBtnTooltipProperty); }
            set { SetValue(MinBtnTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinBtnTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinBtnTooltipProperty =
            DependencyProperty.Register("MinBtnTooltip", typeof(string), typeof(BaseWnd), new PropertyMetadata(null));

        /// <summary>
        /// 最大化按钮的tooltip
        /// </summary>
        public string MaxBtnTooltip
        {
            get { return (string)GetValue(MaxBtnTooltipProperty); }
            set { SetValue(MaxBtnTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxBtnTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxBtnTooltipProperty =
            DependencyProperty.Register("MaxBtnTooltip", typeof(string), typeof(BaseWnd), new PropertyMetadata(null));

        /// <summary>
        /// 关闭按钮的tooltip
        /// </summary>
        public string CloseBtnTooltip
        {
            get { return (string)GetValue(CloseBtnTooltipProperty); }
            set { SetValue(CloseBtnTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseBtnTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseBtnTooltipProperty =
            DependencyProperty.Register("CloseBtnTooltip", typeof(string), typeof(BaseWnd), new PropertyMetadata(null));

        #endregion

        #region 方法
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //设置边框
            wndChrome.CaptionHeight = this.CaptionHeight;
            wndChrome.ResizeBorderThickness = this.BorderSize;
            wndChrome.GlassFrameThickness = new Thickness(1);
            wndChrome.NonClientFrameEdges = NonClientFrameEdges.None;
            wndChrome.CornerRadius = new CornerRadius(0);
            WindowChrome.SetWindowChrome(Window.GetWindow(this), this.wndChrome);

            //注册控件的事件
            minBtn = GetTemplateChild("PART_MINBTN") as Button;
            maxBtn = GetTemplateChild("PART_MAXBTN") as Button;
            closeBtn = GetTemplateChild("PART_CLOSEBTN") as Button;

            if (minBtn != null)
            {
                minBtn.Click += minBtn_Click;
            }

            if (maxBtn != null)
            {
                maxBtn.Click += maxBtn_Click;
            }

            if (closeBtn != null)
            {
                closeBtn.Click += closeBtn_Click;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 获取窗体句柄
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            curHwndSource = HwndSource.FromHwnd(hwnd);
            if (curHwndSource == null)
            {
                return;
            }

            //添加关于菜单
            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = NativeMethods.GetSystemMenu(hwnd, false);

            // Add a separator
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_SEPARATOR, 0, string.Empty);

            // Add the About menu item
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, NativeMethods.SYSMENU_ABOUT_ID, "&About");

            //去除窗体边缘阴影特效
            NativeMethods.RemoveDropShadow(hwnd);

            curHwndSourceHook = new HwndSourceHook(this.WndProc);
            curHwndSource.AddHook(curHwndSourceHook);

            // 计算边框尺寸
            this.CalculateBorderSize();
        }

        /// <summary>
        /// 系统消息处理
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr result = IntPtr.Zero;
            WM_MESSAGE message = (WM_MESSAGE)msg;
            switch (message)
            {
                case WM_MESSAGE.WM_NCLBUTTONDOWN:
                case WM_MESSAGE.WM_NCRBUTTONDOWN:
                    NcButtonDownEventArgs args = new NcButtonDownEventArgs(NcButtonDownEvent, this);
                    this.RaiseEvent(args);
                    break;
                case WM_MESSAGE.WM_SETTINGCHANGE:
                    {
                        // 系统设置变更
                        this.CalculateBorderSize();
                    }
                    break;
                case WM_MESSAGE.WM_DEVICECHANGE:

                    int nEventType = wParam.ToInt32();
                    switch (nEventType)
                    {
                        case 0x8000:// DBT_DEVICEARRIVAL:
                            Console.WriteLine();
                            break;
                        case 0x8004:// DBT_DEVICEREMOVECOMPLETE:
                            Console.WriteLine();
                            break;
                        default:
                            handled = false;
                            return result;
                    }

                    //try
                    //{
                    //    var temp = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_VOLUME));
                    //}
                    //catch
                    //{
                    //    Console.WriteLine();
                    //}



                    break;
                case WM_MESSAGE.WM_SYSCOMMAND:
                    int cmd = wParam.ToInt32();

                    if (cmd == NativeMethods.SYSMENU_ABOUT_ID)
                    {
                        AboutMenuEventArgs args1 = new AboutMenuEventArgs(AboutMenuEvent, this);
                        this.RaiseEvent(args1);
                    }

                    break;
                default:
                    handled = false;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 计算边框尺寸
        /// </summary>
        private void CalculateBorderSize()
        {
            NativeMethods.RECT rc = new NativeMethods.RECT(0, 0, 100, 100);
            NativeMethods.AdjustWindowRectEx(ref rc, WS.WS_THICKFRAME, false, 0);

            double width = Math.Abs(rc.Left);
            SetValue(BorderSizeProperty, new Thickness(width));
        }

        /// <summary>
        /// 关闭窗口是卸载事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (curHwndSource != null)
            {
                curHwndSource.RemoveHook(curHwndSourceHook);
            }
        }

        /// <summary>
        /// 关闭按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            NcButtonDownEventArgs args = new NcButtonDownEventArgs(NcButtonDownEvent, this);
            this.RaiseEvent(args);

            SystemCommands.CloseWindow(Window.GetWindow(this));
        }

        /// <summary>
        /// 最大化按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void maxBtn_Click(object sender, RoutedEventArgs e)
        {
            NcButtonDownEventArgs args = new NcButtonDownEventArgs(NcButtonDownEvent, this);
            this.RaiseEvent(args);

            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(Window.GetWindow(this));
            }
            else
            {
                SystemCommands.MaximizeWindow(Window.GetWindow(this));
            }
        }

        /// <summary>
        /// 最小化按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void minBtn_Click(object sender, RoutedEventArgs e)
        {
            NcButtonDownEventArgs args = new NcButtonDownEventArgs(NcButtonDownEvent, this);
            this.RaiseEvent(args);

            SystemCommands.MinimizeWindow(Window.GetWindow(this));
        }
        #endregion
    }
}
