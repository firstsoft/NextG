using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Framework.Event
{
    /// <summary>
    /// This class for saving a event with multi-type in a list
    /// </summary>
    public class EventToken : IEquatable<EventToken>, IDisposable
    {
        private Action<EventToken> token = null;

        public EventToken(Action<EventToken> _token)
        {
            token = _token;
        }

        public void Dispose()
        {
            if (token != null)
            {
                this.token(this);
            }
        }

        public bool Equals(EventToken other)
        {
            if (other == null) return false;
            return Equals(token, other.token);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.token.GetHashCode();
        }
    }
}
