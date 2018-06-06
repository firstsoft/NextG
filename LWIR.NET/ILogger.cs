using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET
{
    public enum LogType
    {
        Debug,
        Info,
        Error,
        Warn,
    }

    public interface ILogger
    {
        /// <summary>
        /// Write log into file
        /// </summary>
        /// <param name="curLogType"></param>
        /// <param name="typeofClass"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        void WriteLog(LogType curLogType, Type typeofClass, string message, Exception e = null);
    }
}
