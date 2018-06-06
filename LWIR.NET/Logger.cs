using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LWIR.NET
{
    public class Logger : ILogger
    {
        public Logger()
        {
            if (log4net.LogManager.GetRepository().Configured)
            {
                return;
            }

            // my DLL is referenced by web service applications to log SOAP requests before
            // execution is passed to the web method itself, so I load the log4net.config
            // file that resides in the web application root folder
            //日志配置放在外面,方便测试人员或现场人员测试
            string LogXmlPath = System.IO.Directory.GetCurrentDirectory() + "\\LogConfig.xml";

            if (!File.Exists(LogXmlPath))
            {
                log4net.Config.XmlConfigurator.Configure(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Properties.Resources.LogConfig)));
                return;
            }

            try
            {
                FileInfo LogConfigFile = new FileInfo(LogXmlPath);
                log4net.Config.XmlConfigurator.Configure(LogConfigFile);
            }
            catch
            {
                log4net.Config.XmlConfigurator.Configure(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Properties.Resources.LogConfig)));
            }
        }

        /// <summary>
        /// 根据日志级别打印日志
        /// </summary>
        /// <param name="curLogType">打印级别</param>
        /// <param name="typeofClass">打印的日志位置，即类的type</param>
        /// <param name="message">字符串消息</param>
        /// <param name="e">异常</param>
        public void WriteLog(LogType curLogType, Type typeofClass, string message, Exception e = null)
        {
            typeofClass = typeofClass ?? typeof(Logger);
            log4net.ILog log = log4net.LogManager.GetLogger(typeofClass);
            message = message ?? string.Empty;

            switch (curLogType)
            {
                #region
                case LogType.Debug:
                    if (e == null)
                    {
                        log.Debug(message);
                    }
                    else
                    {
                        log.Debug(message, e);
                    }
                    break;
                case LogType.Error:
                    if (e == null)
                    {
                        log.Error(message);
                    }
                    else
                    {
                        log.Error(message, e);
                    }
                    break;
                case LogType.Warn:
                    if (e == null)
                    {
                        log.Warn(message);
                    }
                    else
                    {
                        log.Warn(message, e);
                    }
                    break;
                default:
                    if (e == null)
                    {
                        log.Info(message);
                    }
                    else
                    {
                        log.Info(message, e);
                    }
                    break;
                #endregion
            }
        }
    }
}
