using AMEC.Native;
using AMEC.WH.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMEC.WH.Entity
{
    public class EventInfo
    {
        public int ID { get; set; }
        public EventType Type { get; set; }
        public GroupType Group { get; set; }
        public string Name { get; set; }
        public string Flag { get; set; }
    }

    public class EventDesc
    {
        public string Description1 { get; set; }
    }

    public class LogEvent
    {
        public EventInfo EventInfo { get; set; }
        public EventDesc EventDesc { get; set; }
    }
}
