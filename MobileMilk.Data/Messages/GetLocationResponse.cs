using System.Xml.Serialization;
using System.Collections.Generic;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmGetLocationsResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlArray("locations")]
        [XmlArrayItem("location", typeof(RtmLocationResponse))]
        public List<RtmLocationResponse> Locations { get; set; }
    }

    public class RtmLocationResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("longitude")]
        public string Longitude { get; set; }
        [XmlAttribute("latitude")]
        public string Latitude { get; set; }
        [XmlAttribute("zoom")]
        public string Zoom { get; set; }
        [XmlAttribute("address")]
        public string Address { get; set; }
        [XmlAttribute("viewable")]
        public string Viewable { get; set; }
    }
}
