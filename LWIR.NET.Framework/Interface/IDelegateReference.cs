using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Interface
{
    public interface IDelegateReference
    {
        Delegate Target { get; }
    }
}
