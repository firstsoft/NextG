using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Entity
{
    /// <summary>
    /// 系统风格
    /// </summary>
    public abstract class WS
    {
        public const int WS_OVERLAPPED = 0x00000000;
        public const int WS_POPUP = -2147483648;
        //public const int WS_CHILD = 0x40000000;
        //public const int WS_MINIMIZE = 0x20000000;
        //public const int WS_VISIBLE = 0x10000000;
        //public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        //public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        //public const int WS_VSCROLL = 0x00200000;
        //public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        //public const int WS_GROUP = 0x00020000;
        //public const int WS_TABSTOP = 0x00010000;

        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;

        //public const int WS_TILED = WS_OVERLAPPED;
        //public const int WS_ICONIC = WS_MINIMIZE;
        //public const int WS_SIZEBOX = WS_THICKFRAME;
        //public const int WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;

        // Common Window Styles

        public const int WS_OVERLAPPEDWINDOW =
            (WS_OVERLAPPED |
              WS_CAPTION |
              WS_SYSMENU |
              WS_THICKFRAME |
              WS_MINIMIZEBOX |
              WS_MAXIMIZEBOX);
    }
}
