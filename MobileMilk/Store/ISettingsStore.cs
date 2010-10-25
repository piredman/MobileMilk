using MobileMilk.Data.Entities;

namespace MobileMilk.Store
{
    public interface ISettingsStore
    {
        string AuthorizationFrob { get; set; }
        string AuthorizationToken { get; set; }
        RtmPermissions AuthorizationPermissions { get; set; }
        string AuthorizationPermissionsAsString { get; }

        string UserId { get; set; }
        string UserName { get; set; }
        string FullName { get; set; }
        
        bool LocationServiceAllowed { get; set; }
        bool SubscribeToPushNotifications { get; set; }
    }
}
