using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET
{
    /// <summary>
    /// A global log service
    /// </summary>
    public static class LogService
    {
        private static ILogger log = null;
        public static ILogger Log { get { return log; } }

        public static void SetLogService(ILogger _log)
        {
            log = _log;
        }
    }
}
