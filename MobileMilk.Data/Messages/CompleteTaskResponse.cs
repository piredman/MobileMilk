using System.Xml.Serialization;
using System.Collections.Generic;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmCompleteTaskResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("tasks")]
        public RtmTasksResponse Tasks { get; set; }
    }
}
