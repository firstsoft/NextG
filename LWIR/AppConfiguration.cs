using Microsoft.Practices.ServiceLocation;
using LWIR.NET.Framework.Enum;
using LWIR.NET.Framework.Event;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MEF;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows;
using LWIR.NET;
using LWIR.NET.Framework;

namespace LWIR
{
    /// <summary>
    /// current configuration for current app
    /// </summary>
    public class AppConfiguration : AppBaseConfig
    {
        /// <summary>
        /// init all the window
        /// </summary>
        public override void InitWnd()
        {
            // get the instance of main window
            App.Current.MainWindow = this.Container.GetExportedValue<Shell>();
            this.Container.ComposeParts(App.Current.MainWindow);
        }

        public override void Run()
        {
            // show main window
            App.Current.MainWindow.Show();
        }
    }
}
