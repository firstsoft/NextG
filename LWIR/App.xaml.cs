using LWIR.NET;
using LWIR.NET.Framework.Enum;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MEF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace LWIR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppConfiguration appconfig = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            appconfig = new AppConfiguration();
            appconfig.Init(AppDomain.CurrentDomain.BaseDirectory);

            LogService.Log.WriteLog(LogType.Info, this.GetType(), "LWIR started...");
            appconfig.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogService.Log.WriteLog(LogType.Info, this.GetType(), "LWIR exit...");
            base.OnExit(e);
        }
    }

}
