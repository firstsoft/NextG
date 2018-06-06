using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWIR.NET.Entity
{
    /// <summary>
    /// Value in Axis
    /// </summary>
    public sealed class DataPoint
    {
        private DateTime x = DateTime.Now;
        /// <summary>
        /// Value of X-Axis
        /// </summary>
        public DateTime X
        {
            set { x = value; }
            get { return x; }
        }

        private uint elapsed = 0u;
        public uint Elapsed
        {
            set { elapsed = value; }
            get { return elapsed; }
        }

        private double y = 0.0;
        /// <summary>
        /// Value of Y-Axis
        /// </summary>
        public double Y
        {
            set { y = value; }
            get { return y; }
        }
    }
}
