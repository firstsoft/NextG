using LWIR.NET.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DWORD = System.UInt32;
using WORD = System.UInt16;

namespace LWIR.NET.Framework.Utility
{
    /// <summary>
    /// 本地方法
    /// </summary>
    public class NativeMethods
    {
        static int PROCESS_QUERY_INFORMATION = 0x0400;
        static int PROCESS_VM_READ = 0x0010;
        public const int MF_STRING = 0x0;
        public const int MF_SEPARATOR = 0x800;
        public const int SYSMENU_ABOUT_ID = (int)WM_MESSAGE.WM_USER + 0x001;

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            public int X
            {
                get
                {
                    return this.Left;
                }
            }

            public int Y
            {
                get
                {
                    return this.Top;
                }
            }

            public int Width
            {
                get
                {
                    return this.Right - this.Left;
                }
            }

            public int Height
            {
                get
                {
                    return this.Bottom - this.Top;
                }
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public DWORD dbcv_size;
            public DWORD dbcv_devicetype;
            public DWORD dbcv_reserved;
            public DWORD dbcv_unitmask;
            public WORD dbcv_flags;
        }

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

        /// <summary>
        /// 检索逻辑磁盘的剩余空间
        /// </summary>
        /// <param name="directoryName">磁盘上文件夹路径</param>
        /// <param name="freeBytesAvailable">可用剩余空间字节数</param>
        /// <param name="totalNumberOfBytes">总空间字节数</param>
        /// <param name="totalNumberOfFreeBytes">剩余空间字节数</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

        public const string DOS_DEVICE_NAME = "\\\\.\\aKbFilter";
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_DEVICE_KEYBOARD_FILTER = 0x8000;
        public const uint IOCTL_KEYBOARD_FILTER_SET_EVENT = 1;
        public const uint METHOD_BUFFERED = 0;
        public const uint FILE_ANY_ACCESS = 0;
        public const int FILTER_DATA_LENGTH = 4;
        public const uint IOCTL_KEYBOARD_FILTER_SET_KEY = 4;
        private const ushort KEY_CTRL = 0x1D;
        private const ushort KEY_ALT = 0x38;
        private const ushort KEY_DEL = 0x53;
        private const ushort KEY_NUM = 0x2A;
        private const ushort KEY_TAB = 0x0F;
        private const ushort KEY_NUM_PERIOD = 0x53;
        private const ushort KEY_LWIN = 0x5B;
        private const ushort KEY_RWIN = 0x5C;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        public enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionMaximized = 4
        }

        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        public static ushort[] GetSAS_L()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_CTRL;
            fd[1] = KEY_DEL;
            fd[2] = KEY_ALT;
            fd[3] = KEY_NUM;

            return fd;
        }

        public static ushort[] GetSAS_R()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_CTRL;
            fd[1] = KEY_DEL;
            fd[2] = KEY_ALT;
            fd[3] = KEY_NUM_PERIOD;

            return fd;
        }

        public static ushort[] GetTaskSwitch()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_ALT;
            fd[1] = KEY_TAB;

            return fd;
        }

        public static ushort[] GetWin_L()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_LWIN;

            return fd;
        }

        public static ushort[] GetWin_R()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_RWIN;

            return fd;
        }

        public static ushort[] GetTaskSwitch3D_L()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_LWIN;
            fd[1] = KEY_TAB;

            return fd;
        }

        public static ushort[] GetTaskSwitch3D_R()
        {
            var fd = new ushort[FILTER_DATA_LENGTH];

            fd[0] = KEY_RWIN;
            fd[1] = KEY_TAB;

            return fd;
        }

        public static bool IsDWMEnabled()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
            {
                if (DwmIsCompositionEnabled() == false)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 移除窗口边缘背景
        /// </summary>
        /// <param name="hWnd"></param>
        public static void RemoveDropShadow(IntPtr hWnd)
        {
            // Enable NcRenderingPolicy
            try
            {
                int attrValue = 2;
                int result = DwmSetWindowAttribute(hWnd, 2, ref attrValue, 4);

                if (result == 0)
                {
                    // Extend DwmFrame
                    DwmMargins margins = new DwmMargins(false);
                    DwmExtendFrameIntoClientArea(hWnd, ref margins);
                }
            }
            catch
            {

            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmMargins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;

            public DwmMargins(bool hasDropDownShadow)
            {
                this.cxLeftWidth = this.cxRightWidth = this.cyTopHeight = this.cyBottomHeight = hasDropDownShadow ? -1 : 0;
            }
        }

        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref DwmMargins m);

        [DllImport("DwmApi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);

        /// <summary>
        /// Get fullfile by processId
        /// </summary>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        public static string GetFullFileByProcessId(int dwProcessId)
        {
            IntPtr pHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, dwProcessId);

            if (pHandle == IntPtr.Zero)
                return null;

            StringBuilder sb = new StringBuilder(260);
            GetModuleFileNameEx(pHandle, IntPtr.Zero, sb, 260);

            CloseHandle(pHandle);
            return sb.ToString();
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetCapture();

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);

    }
}
