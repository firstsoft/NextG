using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LWIR.NET.Entity
{
    public sealed class BrushInfo
    {
        /// <summary>
        /// The name of the brush
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current brush
        /// </summary>
        public SolidColorBrush CurBrush { get; set; }
    }
}
