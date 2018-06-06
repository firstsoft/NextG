using Microsoft.Win32.SafeHandles;
using LWIR.NET.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LWIR.NET.Common.Utility
{
    public class LockScreenUtility : IDisposable
    {
        private SafeFileHandle m_file = null;
        private ManualResetEvent m_event = null;
        private bool m_isSetFilterData = false; // Indicate if filter data is sent to kernel

        /// <summary>
        /// Initialized the class
        /// </summary>
        /// <param name="isDriverLock">True for locked by driver, otherwise by software</param>
        public LockScreenUtility()
        {
            var file = NativeMethods.CreateFile(NativeMethods.DOS_DEVICE_NAME,
                      NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            m_file = new SafeFileHandle(file, true);

            if (m_file.IsInvalid == false)
                m_event = new ManualResetEvent(false);
        }

        /// <summary>
        /// Lock the screen
        /// </summary>
        public void Lock()
        {
            if (m_file.IsInvalid)
            {
                return;
            }

            uint bytesReturned = 0;
            if (m_isSetFilterData == false)
            {
                #region
                unsafe
                {
                    var ioCtrl = NativeMethods.CTL_CODE(
                        NativeMethods.FILE_DEVICE_KEYBOARD_FILTER,
                        NativeMethods.IOCTL_KEYBOARD_FILTER_SET_EVENT,
                        NativeMethods.METHOD_BUFFERED,
                        NativeMethods.FILE_ANY_ACCESS
                        );

                    int handle = m_event.SafeWaitHandle.DangerousGetHandle().ToInt32();
                    int* pHandle = &handle;

                    var ret = NativeMethods.DeviceIoControl(m_file.DangerousGetHandle(), ioCtrl, (IntPtr)(pHandle), 4, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
                    LockGroupKeys();

                    m_isSetFilterData = true;
                }
                #endregion

                m_event.Set();
            }
        }

        private bool LockGroupKeys()
        {
            unsafe
            {
                List<ushort[]> listFd = new List<ushort[]>();
                listFd.Add(NativeMethods.GetSAS_L());
                listFd.Add(NativeMethods.GetSAS_R());
                listFd.Add(NativeMethods.GetTaskSwitch());
                listFd.Add(NativeMethods.GetTaskSwitch3D_L());
                listFd.Add(NativeMethods.GetTaskSwitch3D_R());
                listFd.Add(NativeMethods.GetWin_L());
                listFd.Add(NativeMethods.GetWin_R());

                int size = listFd.Count * NativeMethods.FILTER_DATA_LENGTH;
                ushort* fd = stackalloc ushort[size];
                ushort* tmp = fd;

                for (int i = 0; i < listFd.Count; ++i)
                {
                    for (int j = 0; j < NativeMethods.FILTER_DATA_LENGTH; ++j, ++tmp)
                    {
                        *tmp = listFd[i][j];
                    }
                }

                var ioCtrl = NativeMethods.CTL_CODE(
                    NativeMethods.FILE_DEVICE_KEYBOARD_FILTER,
                    NativeMethods.IOCTL_KEYBOARD_FILTER_SET_KEY,
                    NativeMethods.METHOD_BUFFERED,
                    NativeMethods.FILE_ANY_ACCESS
                    );

                uint bytesReturned = 0;
                return NativeMethods.DeviceIoControl(m_file.DangerousGetHandle(), ioCtrl, (IntPtr)(fd), (uint)size * 2, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Unlock the screen
        /// </summary>
        public void Unlock()
        {
            if (m_file.IsInvalid == false)
            {
                m_event.Reset();
            }
        }

        /// <summary>
        /// Dispose all the object
        /// </summary>
        public void Dispose()
        {
            if (this.m_event != null)
            {
                m_event.Reset();
                this.m_event.Close();
                this.m_event = null;
            }

            if (this.m_file != null)
            {
                this.m_file.Close();
                this.m_file = null;
            }

            m_isSetFilterData = false;

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
