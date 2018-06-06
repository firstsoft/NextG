using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Entity
{
    /// <summary>
    /// 系统消息
    /// </summary>
    public enum WM_MESSAGE
    {
        WM_CREATE = 0x0001,
        WM_SIZE = 0x0005,
        WM_PAINT = 0x000F,
        WM_ERASEBKGND = 0x0014,
        WM_SETTINGCHANGE = 0x001A,
        WM_GETMINMAXINFO = 0x0024,
        WM_GETICON = 0x007F,
        WM_NCCREATE = 0x0081,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN= 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_SYSCOMMAND = 0x0112,
        WM_LBUTTONUP = 0x0202,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DWMCOMPOSITIONCHANGED = 0x031E,
        WM_DWMNCRENDERINGCHANGED = 0x031F,
        WM_DEVICECHANGE = 0x0219,
        WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
        WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
        WM_GETTITLEBARINFOEX = 0x033F,
        WM_USER = 0x0400,
        /// <summary>
        /// Notify that machine power state is changing
        /// </summary>
        WM_POWERBROADCAST = 0x218,

        //非客户区重绘
        WM_UNINITMENUPOPUP = 0x125,
        WM_NCUAHDRAWCAPTION = 0x00AE,
        WM_NCUAHDRAWFRAME = 0x00AF,
    }

}
