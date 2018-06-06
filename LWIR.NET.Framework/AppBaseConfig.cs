using LWIR.NET.Entity;
using LWIR.NET.Framework.Core;
using LWIR.NET.Framework.Entity;
using LWIR.NET.Framework.Event;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MEF;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace LWIR.NET.Framework
{
    public abstract class AppBaseConfig
    {
        // parts collection
        protected AggregateCatalog AggregateCatalog { get; set; }

        // container for pplugins
        protected CompositionContainer Container { get; set; }

        // log for app
        protected ILogger log { get; set; }

        // region manager for app
        protected RegionManager Regionmanager { get; set; }

        protected EventAggregator EventAggregator { get; set; }

        /// <summary>
        /// init all the configurations
        /// </summary>
        /// <param name="dirPath"></param>
        public void Init(string dirPath)
        {
            // load sysytem params
            LoadSysParams();

            // create instance for app
            this.log = new Logger();
            this.AggregateCatalog = new AggregateCatalog();
            this.Regionmanager = new RegionManager();
            this.EventAggregator = new EventAggregator();


            // find plugins
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));

            if (Directory.Exists(dirPath))
            {
                DirectoryCatalog directoryCatalog = new DirectoryCatalog(dirPath);
                this.AggregateCatalog.Catalogs.Add(directoryCatalog);
            }

            // create container by parts
            this.Container = new CompositionContainer(this.AggregateCatalog);

            // register service
            this.Container.ComposeExportedValue<ILogger>(this.log);
            this.Container.ComposeExportedValue<IServiceLocator>(new MefServiceLocator(this.Container));
            this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
            this.Container.ComposeExportedValue<IRegionManager>(this.Regionmanager);
            this.Container.ComposeExportedValue<IEventAggregator>(this.EventAggregator);

            // configure service
            IServiceLocator serviceLocator = this.Container.GetExportedValue<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            LogService.SetLogService(this.log);

            // compose all the parts
            try
            {
                this.Container.ComposeParts(this);
            }
            catch (Exception e)
            {
                log.WriteLog(LogType.Error, this.GetType(), null, e);
            }

            // get the instance of main window
            InitWnd();
        }

        public virtual void InitWnd()
        {
        }

        public virtual void Run()
        {
        }

        internal void LoadSysParams()
        {
            PropertyInfo[] infos = typeof(Brushes).GetProperties();

            List<BrushInfo> BrushArray = new List<BrushInfo>();
            foreach (PropertyInfo f in infos)
            {
                SolidColorBrush brush = (SolidColorBrush)f.GetValue(null, null);
                brush.Freeze();

                if (brush.Equals(Brushes.Transparent))
                {
                    continue;
                }

                BrushInfo bi = new BrushInfo();
                bi.Name = f.Name;
                bi.CurBrush = brush;

                BrushArray.Add(bi);
            }

            SystemParams.AllBrushes = BrushArray.ToArray();
        }
    }
}
