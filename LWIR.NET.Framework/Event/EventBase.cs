using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Event
{
    ///<summary>
    /// Defines a base class to publish and subscribe to events.
    ///</summary>
    public abstract class EventBase
    {
        protected readonly List<IEvent> actions = new List<IEvent>();

        /// <summary>
        /// Subscribe a event
        /// </summary>
        /// <param name="curAction"></param>
        protected virtual EventToken InnerSubscribe(IEvent curAction)
        {
            if (curAction == null) throw new System.ArgumentNullException("curAction");

            EventToken token = new EventToken(this.InnerUnsubscribe);

            lock (actions)
            {
                curAction.token = token;
                actions.Add(curAction);
            }

            return token;
        }

        /// <summary>
        /// publish data to event
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual void InnerPublish(params object[] arguments)
        {
            List<Action<object[]>> excutionActions = GetExcutionActions();
            foreach (var curAction in excutionActions)
            {
                curAction(arguments);
            }
        }

        /// <summary>
        /// Unsubscribe the specify event with the specify token
        /// </summary>
        /// <param name="token">A event attact to a specify token</param>
        public virtual void InnerUnsubscribe(EventToken token)
        {
            lock (actions)
            {
                IEvent event1 = actions.FirstOrDefault(act => act.token == token);
                if (event1 != null)
                {
                    actions.Remove(event1);
                }
            }
        }

        /// <summary>
        /// Get all the actions which match with the args
        /// </summary>
        /// <returns></returns>
        private List<Action<object[]>> GetExcutionActions()
        {
            List<Action<object[]>> returnList = new List<Action<object[]>>();

            lock (this.actions)
            {
                for (var i = this.actions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> listItem = this.actions[i].GetExcutionAction();

                    if (listItem == null)
                    {
                        // remove the invalid actions
                        this.actions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }
    }
}
