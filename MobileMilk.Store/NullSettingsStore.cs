using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MobileMilk.Store
{
    public class NullSettingsStore : ISettingsStore
    {
        public string AuthorizationFrob { get; set; }

        public string AuthorizationToken { get; set; }

        public Model.Permissions AuthorizationPermissions { get; set; }

        public string AuthorizationPermissionsAsString { get { return string.Empty; } }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public bool LocationServiceAllowed { get; set; }

        public bool SubscribeToPushNotifications { get; set; }
    }
}
