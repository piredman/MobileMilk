namespace MobileMilk.Model
{
    public class Authorization
    {
        public string Token { get; set; }
        public Permissions Permissions { get; set; }
        public User User { get; set; }
    }
}
