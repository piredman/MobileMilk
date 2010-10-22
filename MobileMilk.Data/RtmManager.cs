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
using System.Xml.Linq;
using System.Collections.Generic;

namespace MobileMilk.Data
{
    public delegate void GetAuthorizationUrlDelegate(string url);

    public class RtmManager
    {
        private string _frob;

        public void GetAuthorizationUrl(GetAuthorizationUrlDelegate callback)
        {
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var url = rtmRequestBuilder.GetFrobRequest();

            this.GetRequest(url, ((object sender, DownloadStringCompletedEventArgs e) => {
                GetRequestComplete(sender, e, callback);
            }));
        }
        
        private void GetRequest(string url, DownloadStringCompletedEventHandler callback)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += callback;
            client.DownloadStringAsync(new Uri(url));
        }

        private void GetRequestComplete(object sender, DownloadStringCompletedEventArgs e, 
            GetAuthorizationUrlDelegate callback)
        {
            if (e.Error != null)
                return;

            string frob = string.Empty;

            XElement xml = XElement.Parse(e.Result);
            IEnumerable<XElement> descendents = xml.Descendants("frob");
            foreach (XElement d in descendents)
            {
                frob = d.Value;
            }

            _frob = frob;
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var authorizationUrl = rtmRequestBuilder.GetAuthenticationUrl(_frob, AuthenticationPermissions.Delete);

            callback(authorizationUrl);
        }
    }
}
