using System.Xml.Serialization;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmCreateTimelineResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("timeline")]
        public string Timeline { get; set; }
    }
}
