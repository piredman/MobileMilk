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

namespace MobileMilk.Data
{
    public interface IRtmManager
    {
        string Frob { get; set; }
        string Token { get; set; }

        void GetAuthorizationUrl(GetUrlDelegate callback);
        void GetAuthorizationToken(GetTokenDelegate callback);
        void GetAuthorization(GetAuthorizationDelegate callback);
        void CreateTimeline(GetTimelineDelegate callback);
        void GetTasksList(GetTasksDelegate callback);
    }
}
