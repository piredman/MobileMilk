namespace MobileMilk.Data.Entities
{
    public class RtmAuthorization
    {
        public string Token { get; set; }
        public RtmPermissions Permissions { get; set; }
        public RtmUser User { get; set; }
    }
}
