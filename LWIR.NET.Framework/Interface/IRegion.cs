using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Interface
{
    public interface IRegion : IDisposable
    {
        /// <summary>
        /// Get view by contact name
        /// </summary>
        /// <param name="contactName">specify contact name</param>
        /// <returns></returns>
        object GetView(string contactName);

        /// <summary>
        /// navigate to specify view
        /// </summary>
        /// <param name="contactName">specify contact name of view</param>
        void NavigateTo(string contactName);

        /// <summary>
        /// Remove specify view
        /// </summary>
        /// <param name="contactName"></param>
        void RemoveView(string contactName);
    }
}
