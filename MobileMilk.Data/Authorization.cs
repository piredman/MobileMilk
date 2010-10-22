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
    public class Authorization
    {
        private const string ApiKey = "f2b852927319553de9c694d1debe96d6";
        private const string SharedSecret = "ae078deb376100d7";

        public string GetFrob()
        {
            IronCow.Rtm rtm = new IronCow.Rtm(ApiKey, SharedSecret);
            return rtm.GetFrob();
        }

        public string GetAuthorizationPageUrl(string frob)
        {
            IronCow.Rtm rtm = new IronCow.Rtm(ApiKey, SharedSecret);
            return rtm.GetAuthenticationUrl(frob, IronCow.AuthenticationPermissions.Write);
        }

        public string GetAuthorizationToken(string frob)
        {
            IronCow.Rtm rtm = new IronCow.Rtm(ApiKey, SharedSecret);
            return rtm.GetToken(frob);
        }
    }
}
