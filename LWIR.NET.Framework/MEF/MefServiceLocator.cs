using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.MEF
{
    public class MefServiceLocator : ServiceLocatorImplBase
    {
        private readonly CompositionContainer compositionContainer;

        public MefServiceLocator(CompositionContainer compositionContainer1)
        {
            this.compositionContainer = compositionContainer1;
        }

        /// <summary>
        /// Get all the instances
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            List<object> instances = new List<object>();

            IEnumerable<Lazy<object, object>> exports = this.compositionContainer.GetExports(serviceType, null, null);
            if (exports != null)
            {
                instances.AddRange(exports.Select(export => export.Value));
            }

            return instances;
        }

        /// <summary>
        /// Get the instance by specify type and contractname
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string contractName)
        {
            IEnumerable<Lazy<object, object>> exports = this.compositionContainer.GetExports(serviceType, null, contractName);
            if ((exports != null) && (exports.Count() > 0))
            {
                // If there is more than one value, this will throw an InvalidOperationException, 
                // which will be wrapped by the base class as an ActivationException.
                return exports.Single().Value;
            }

            throw new ActivationException(
                this.FormatActivationExceptionMessage(new CompositionException("Export not found"), serviceType, contractName));
        }

        protected Type GetPartType(string contractName)
        {
            return null;
            //this.compositionContainer.Catalog.Parts.FirstOrDefault(p=>p.ExportDefinitions.);
        }
    }
}
