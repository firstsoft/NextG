using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Event
{
    public class CompositeEvent<TParam> : EventBase
    {
        /// <summary>
        /// Subscribe a event
        /// </summary>
        /// <param name="curAction"></param>
        public void Subscribe(Action<TParam> action, bool keepAlive = false)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepAlive);

            if (action == null) throw new System.ArgumentNullException("action");

            base.InnerSubscribe(new Event<TParam>(actionReference));
        }

        public void Publish(TParam arguments)
        {
            base.InnerPublish(arguments);
        }

        public void Unsubscribe(Action<TParam> action)
        {
            lock(this.actions)
            {
                IEvent event1 = this.actions.Cast<Event<TParam>>().FirstOrDefault(e=>e.Action==action);

                if(event1!=null)
                {
                    this.actions.Remove(event1);
                }
            }
        }
    }
}
