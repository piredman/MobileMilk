using System.Xml.Serialization;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmUpdateTaskResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("transaction")]
        public RtmTasksResponse Transaction { get; set; }

        [XmlElement("tasks")]
        public RtmTasksResponse Tasks { get; set; }
    }

    public class RtmTransaction
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("undoable")]
        public string Undoable { get; set; }
    }
}
