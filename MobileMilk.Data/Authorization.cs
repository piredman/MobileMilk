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

        public void Authorize()
        {
            IronCow.Rtm rtm = new IronCow.Rtm(ApiKey, SharedSecret);
            string frob = rtm.GetFrob();
            
            //TODO: Figure this out
            //System.Diagnostics.Process.Start(rtm.GetAuthenticationUrl(frob, IronCow.AuthenticationPermissions.Write));   // Run the default browser to open the "authenticate" page on RTM's website.
            Console.WriteLine("Press enter to continue after you have authenticated in the browser...");
            Console.ReadLine();
            
            rtm.AuthToken = rtm.GetToken(frob);    // Get the auth-token with the same frob we got earlier.
            Console.WriteLine("Successfully authenticated! Got token: " + rtm.AuthToken);
        }
    }
}
