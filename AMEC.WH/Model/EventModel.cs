using AMEC.Native;
using AMEC.WH.Entity;
using AMEC.WH.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace AMEC.WH.Model
{
    public class EventModel
    {
        private static readonly Lazy<EventModel> lazy = new Lazy<EventModel>(() => new EventModel());
        private List<LogEvent> logEvents = new List<LogEvent>();
        private Dictionary<string, LogEvent> logDic = new Dictionary<string, LogEvent>();

        public static EventModel Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void Load()
        {
            logEvents.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Event.xml"));

            XmlNode ASEventNode = doc.SelectSingleNode("ASEvent");
            XmlNodeList LogEventList = ASEventNode.SelectNodes("LogEvent");

            foreach (XmlNode logNode in LogEventList)
            {
                LogEvent logEvent = new LogEvent();
                logEvent.EventInfo = new EventInfo();
                logEvent.EventDesc = new EventDesc();

                XmlNode EventInfoNode = logNode.SelectSingleNode("EventInfo");
                logEvent.EventInfo.ID = int.Parse(EventInfoNode.Attributes["ID"].Value);
                logEvent.EventInfo.Type = (EventType)System.Enum.Parse(typeof(EventType), EventInfoNode.Attributes["Type"].Value);
                logEvent.EventInfo.Group = (GroupType)System.Enum.Parse(typeof(GroupType), EventInfoNode.Attributes["Group"].Value);
                logEvent.EventInfo.Name = EventInfoNode.Attributes["Name"].Value;
                logEvent.EventInfo.Flag = EventInfoNode.Attributes["Flag"].Value;

                XmlNode EventDescNode = logNode.SelectSingleNode("EventDesc");
                XmlNode Description1Node = EventDescNode.SelectSingleNode("Description1");
                logEvent.EventDesc.Description1 = Description1Node.FirstChild == null ? string.Empty : Description1Node.FirstChild.Value;

                string key = string.Format("{0}_{1}_{2}", logEvent.EventInfo.ID, logEvent.EventInfo.Type, logEvent.EventInfo.Group);
                if (!logDic.ContainsKey(key))
                {
                    logDic.Add(key, logEvent);
                    logEvents.Add(logEvent);
                }
            }
        }

        public string GetEventMessage(int eventId, EventType eventType, GroupType groupType)
        {
            string key = string.Format("{0}_{1}_{2}", eventId, eventType, groupType);

            if (logDic.ContainsKey(key))
            {
                return logDic[key].EventDesc.Description1;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
