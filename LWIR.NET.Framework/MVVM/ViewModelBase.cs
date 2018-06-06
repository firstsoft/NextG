using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.MVVM
{
    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public abstract void Dispose();
    }
}
