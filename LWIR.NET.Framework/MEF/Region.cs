using Microsoft.Practices.ServiceLocation;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.MEF
{
    public class Region : IRegion
    {
        private Dictionary<string, DependencyObject> dic = null;
        private PropertyInfo[] propertiesOfContainor = null;

        private string name = string.Empty;
        /// <summary>
        /// Name of region
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        private WeakReference targetElement = null;
        /// <summary>
        /// container of current region
        /// </summary>
        public DependencyObject TargetElement
        {
            get { return targetElement == null ? null : targetElement.Target as DependencyObject; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="regionName">name of region</param>
        /// <param name="targetElement1">container of current region</param>
        public Region(string regionName, DependencyObject targetElement1)
        {
            this.name = regionName;
            this.targetElement = new WeakReference(targetElement1);
            propertiesOfContainor = targetElement1.GetType().GetProperties();

            if (propertiesOfContainor == null || propertiesOfContainor.Length == 0)
            {
                throw new InvalidOperationException("targetElement1 is not a containor.");
            }

            dic = new Dictionary<string, DependencyObject>();
        }

        private void RegisterView(string contactName, DependencyObject view)
        {
            if (contactName == null || view == null)
                throw new InvalidOperationException("contactName, viewType and view should not be empty.");

            lock (this.dic)
            {
                if (!this.dic.ContainsKey(contactName))
                {
                    this.dic.Add(contactName, view);
                }
            }
        }

        public object GetView(string contactName)
        {
            lock (this.dic)
            {
                return this.dic.ContainsKey(contactName) ? this.dic[contactName] : null;
            }
        }

        public void NavigateTo(string contactName)
        {
            object view = this.GetView(contactName);

            if (view == null)
            {
                view = ServiceLocator.Current.GetInstance<object>(contactName);
            }

            if (view == null)
            {
                return;
            }

            this.RegisterView(contactName, view as DependencyObject);

            // show view
            if (propertiesOfContainor.Any(x => x.Name.Equals("Content")))
            {
                propertiesOfContainor.FirstOrDefault(p => p.Name.Equals("Content")).SetValue(this.TargetElement, view, null);
            }
            else if (propertiesOfContainor.Any(x => x.Name.Equals("Child")))
            {
                propertiesOfContainor.FirstOrDefault(p => p.Name.Equals("Child")).SetValue(this.TargetElement, view, null);
            }
            //else if (propertiesOfContainor.Any(x => x.Name.Equals("Children")))
            //{

            //}
            //else if (propertiesOfContainor.Any(x => x.Name.Equals("ItemSource")))
            //{

            //}
            else
            {
                throw new InvalidOperationException("Current region is not a containor to host the specify view.");
            }
        }

        public void RemoveView(string contactName)
        {
            lock (this.dic)
            {
                if (this.dic.ContainsKey(contactName))
                {
                    FrameworkElement d = this.dic[contactName] as FrameworkElement;

                    if (d != null && d.DataContext != null)
                    {
                        (d.DataContext as ViewModelBase).Dispose();
                    }

                    this.dic.Remove(contactName);
                }
            }
        }

        public void Dispose()
        {
            targetElement = null;

            lock (this.dic)
            {
                this.dic.Keys.ToList().ForEach(contactName => this.RemoveView(contactName));
                dic.Clear();
            }
        }
    }
}
