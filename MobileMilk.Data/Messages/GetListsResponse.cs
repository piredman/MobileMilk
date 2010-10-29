using System.Xml.Serialization;
using System.Collections.Generic;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmGetListsResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("lists")]
        public List<RtmListResponse> Lists { get; set; }
    }

    public class RtmListResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("deleted")]
        public string Deleted { get; set; }
        [XmlAttribute("locked")]
        public string Locked { get; set; }
        [XmlAttribute("archived")]
        public string Archived { get; set; }
        [XmlAttribute("position")]
        public string Position { get; set; }
        [XmlAttribute("smart")]
        public string Smart { get; set; }

        [XmlElement("filter")]
        public List<string> Filters { get; set; }
    }
}
