using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LWIR.NET.Framework.Interface
{
    public interface IRegionManager : IDisposable
    {
        /// <summary>
        /// Add region to manager
        /// </summary>
        /// <param name="regionName">name of region</param>
        /// <param name="targetElement">target element as container</param>
        void AddRegion(string regionName, DependencyObject targetElement);

        /// <summary>
        /// Remove region from collection
        /// </summary>
        /// <param name="regionName"></param>
        void RemoveRegion(string regionName);

        /// <summary>
        /// Get region from collection
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        IRegion GetRegion(string regionName);

        /// <summary>
        /// show specify view in the specify region
        /// </summary>
        /// <param name="regionName">specify region</param>
        /// <param name="contactName">specify view</param>
        void NavigateTo(string regionName, string contactName);
    }
}
