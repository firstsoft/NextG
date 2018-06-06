using LWIR.NET.Framework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Interface
{
    public interface IEvent
    {
        /// <summary>
        /// The token for Unsubscribe
        /// </summary>
        EventToken token { get; set; }

        /// <summary>
        /// Get all the actions which match with the args
        /// </summary>
        /// <returns></returns>
        Action<object[]> GetExcutionAction();
    }
}
