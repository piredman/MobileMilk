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
    public class RtmAuthorizationService : IRtmAuthorizationService
    {
        /*
        public string GetAuthorizationFrob()
        {
            MobileMilk.Data.Rtm rtm = new MobileMilk.Data.Rtm(Constants.ApiKey, Constants.SharedSecret);
            return rtm.GetFrob();
        }

        public string GetAuthorizationPageUrl(string authorizationFrob)
        {
            MobileMilk.Data.Rtm rtm = new MobileMilk.Data.Rtm(Constants.ApiKey, Constants.SharedSecret);
            return rtm.GetAuthenticationUrl(authorizationFrob, MobileMilk.Data.AuthenticationPermissions.Write);
        }

        public string GetAuthorizationToken(string authorizationFrob)
        {
            MobileMilk.Data.Rtm rtm = new MobileMilk.Data.Rtm(Constants.ApiKey, Constants.SharedSecret);
            return rtm.GetToken(authorizationFrob);
        }

        public bool CheckAuthorizationToken(string authorizationToken)
        {
            MobileMilk.Data.Authentication authentication;

            MobileMilk.Data.Rtm rtm = new MobileMilk.Data.Rtm(Constants.ApiKey, Constants.SharedSecret);
            return rtm.CheckToken(authorizationToken, out authentication);
        }
        */
        #region IRtmAuthorizationService Members

        public string GetAuthorizationFrob()
        {
            throw new NotImplementedException();
        }

        public string GetAuthorizationPageUrl(string authorizationFrob)
        {
            throw new NotImplementedException();
        }

        public string GetAuthorizationToken(string authorizationFrob)
        {
            throw new NotImplementedException();
        }

        public bool CheckAuthorizationToken(string authorizationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
