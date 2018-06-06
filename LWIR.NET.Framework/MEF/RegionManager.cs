using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.MEF
{
    public class RegionManager : IRegionManager
    {
        private Dictionary<string, IRegion> dic = null;

        public RegionManager()
        {
            dic = new Dictionary<string, IRegion>();
        }

        public void AddRegion(string regionName, DependencyObject targetElement)
        {
            if (string.IsNullOrEmpty(regionName) || targetElement == null)
                throw new InvalidCastException();

            lock (this.dic)
            {
                if (!this.dic.ContainsKey(regionName))
                {
                    this.dic.Add(regionName, new Region(regionName, targetElement));
                }
            }
        }

        public void RemoveRegion(string regionName)
        {
            lock (this.dic)
            {
                if (this.dic.ContainsKey(regionName))
                {
                    this.dic[regionName].Dispose();
                    this.dic.Remove(regionName);
                }
            }
        }

        public IRegion GetRegion(string regionName)
        {
            lock (this.dic)
            {
                return this.dic.ContainsKey(regionName) ? this.dic[regionName] : null;
            }
        }

        public void NavigateTo(string regionName, string contactName)
        {
            if (string.IsNullOrEmpty(regionName) || string.IsNullOrEmpty(contactName))
                throw new InvalidOperationException("regionName and contactName should not be empty.");

            IRegion region = this.GetRegion(regionName);

            if (region == null)
                throw new InvalidOperationException("Region does not exist.");

            region.NavigateTo(contactName);
        }

        public void Dispose()
        {
            lock (this.dic)
            {
                this.dic.Values.ToList().ForEach(r => r.Dispose());
                this.dic.Clear();
            }
        }
    }
}
