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
    public interface ISettingsStore
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool LocationServiceAllowed { get; set; }
        bool SubscribeToPushNotifications { get; set; }
    }
}
