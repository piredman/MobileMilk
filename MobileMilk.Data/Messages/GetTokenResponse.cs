using System.Xml.Serialization;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmGetTokenResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("auth")]
        public AuthorizationResponse Authorization { get; set; }
    }

    public class AuthorizationResponse
    {
        [XmlElement("token")]
        public string Token { get; set; }

        [XmlElement("perms")]
        public string Permissions { get; set; }

        [XmlElement("user")]
        public RtmUserResponse User { get; set; }
    }

    public class RtmUserResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("username")]
        public string UserName { get; set; }

        [XmlAttribute("fullname")]
        public string FullName { get; set; }
    }
}
