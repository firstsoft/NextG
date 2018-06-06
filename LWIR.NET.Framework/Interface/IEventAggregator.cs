using LWIR.NET.Framework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Interface
{
    /// <summary>
    /// Defines an interface to get instances of an event type.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Get specify event by type from pool
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <returns></returns>
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
