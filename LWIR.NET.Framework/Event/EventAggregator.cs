using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Event
{
    /// <summary>
    /// Implements <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        /// <summary>
        /// Get specify event by type from pool
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <returns></returns>
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (events)
            {
                if (!this.events.ContainsKey(typeof(TEventType)))
                {
                    TEventType newEvent = new TEventType();
                    this.events.Add(typeof(TEventType), newEvent);
                }

                return this.events[typeof(TEventType)] as TEventType;
            }
        }
    }
}
