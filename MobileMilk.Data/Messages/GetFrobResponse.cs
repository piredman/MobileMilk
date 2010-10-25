using System.Xml.Serialization;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmGetFrobResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("frob")]
        public string Frob { get; set; }
    }
}
