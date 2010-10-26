using MobileMilk.Model;

namespace MobileMilk.Store
{
    public interface ISettingsStore
    {
        string AuthorizationFrob { get; set; }
        string AuthorizationToken { get; set; }
        Permissions AuthorizationPermissions { get; set; }
        string AuthorizationPermissionsAsString { get; }

        string UserId { get; set; }
        string UserName { get; set; }
        string FullName { get; set; }
        
        bool LocationServiceAllowed { get; set; }
        bool SubscribeToPushNotifications { get; set; }
    }
}
