using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Event
{
    public class Event<TParam> : IEvent
    {
        private readonly IDelegateReference actionReference;

        public Event(IDelegateReference action, bool keepAlive = false)
        {
            if (action == null)
                throw new ArgumentNullException("actionReference");

            if (!(action.Target is Action<TParam>))
                throw new ArgumentException(String.Format("Type of {0} is error.", typeof(Action<TParam>).FullName));

            actionReference = action;
        }

        /// <summary>
        /// Current action which Event can excute
        /// </summary>
        public Action<TParam> Action
        {
            get { return (Action<TParam>)actionReference.Target; }
        }

        /// <summary>
        /// Get all the actions which match with the args
        /// </summary>
        /// <returns></returns>
        public Action<object[]> GetExcutionAction()
        {
            if (this.Action != null)
            {
                return args =>
                    {
                        TParam arg = default(TParam);
                        if (args != null && args.Length > 0 && args[0] != null)
                        {
                            arg = (TParam)args[0];
                        }

                        this.Action(arg);
                    };
            }

            return null;
        }

        public EventToken token { get; set; }
    }
}
