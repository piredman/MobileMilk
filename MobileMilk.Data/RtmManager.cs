using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using MobileMilk.Data.Common;
using MobileMilk.Data.Entities;
using MobileMilk.Data.Messages;

namespace MobileMilk.Data
{
    public delegate void GetUrlDelegate(string url);
    public delegate void GetTokenDelegate(string token);
    public delegate void GetAuthorizationDelegate(RtmAuthorization authorization);
    public delegate void GetTimelineDelegate(string timeline);

    public class RtmManager : IRtmManager
    {
        #region Members

        private string _frob;
        private string _token;
        private string _timeline;

        #endregion Members

        #region Properties

        public string Frob { get { return _frob; } set { _frob = value; } }
        public string Token { get { return _token; } set { _token = value; } }
        public string Timeline { get { return _timeline; } set { _timeline = value; } }

        #endregion Properties

        #region Methods

        public void GetAuthorizationUrl(GetUrlDelegate callback)
        {
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var url = rtmRequestBuilder.GetFrobRequest();

            this.GetRequest(url, ((object sender, DownloadStringCompletedEventArgs e) =>
                GetUrlRequestComplete(sender, e, callback)
            ));
        }

        public void GetAuthorizationToken(GetTokenDelegate callback)
        {
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var url = rtmRequestBuilder.GetTokenRequest(_frob);

            this.GetRequest(url, ((object sender, DownloadStringCompletedEventArgs e) =>
                GetTokenRequestComplete(sender, e, callback)
            ));
        }

        public void GetAuthorization(GetAuthorizationDelegate callback)
        {
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var url = rtmRequestBuilder.GetCheckTokenRequest(_token);

            this.GetRequest(url, ((object sender, DownloadStringCompletedEventArgs e) =>
                GetAuthorizationRequestComplete(sender, e, callback)
            ));
        }

        public void GetTimeline(GetTimelineDelegate callback)
        {
            var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
            var url = rtmRequestBuilder.GetTimelineRequest();

            this.GetRequest(url, ((object sender, DownloadStringCompletedEventArgs e) =>
                GetTimelineRequestComplete(sender, e, callback)
            ));
        }

        #region Private Methods

        private void GetUrlRequestComplete(object sender, DownloadStringCompletedEventArgs e,
            GetUrlDelegate callback)
        {
            string authorizationUrl = null;

            try
            {
                if (e.Error != null)
                    return;

                var responseXml = XElement.Parse(e.Result);
                var reader = new StringReader(responseXml.ToString());

                var serializer = new XmlSerializer(typeof(RtmGetFrobResponse));
                var response = serializer.Deserialize(reader) as RtmGetFrobResponse;
                if (null == response)
                    return;

                _frob = response.Frob;

                var rtmRequestBuilder = new RtmRequestBuilder(Constants.ApiKey, Constants.SharedSecret);
                authorizationUrl = rtmRequestBuilder.GetAuthenticationUrl(_frob, RtmPermissions.delete);
            }
            catch (Exception)
            {
                //TODO: Error handling
                throw;
            }
            finally
            {
                callback(authorizationUrl);
            }
        }

        private void GetTokenRequestComplete(object sender, DownloadStringCompletedEventArgs e,
            GetTokenDelegate callback)
        {
            try
            {
                if (e.Error != null)
                    return;

                var responseXml = XElement.Parse(e.Result);
                var reader = new StringReader(responseXml.ToString());

                var serializer = new XmlSerializer(typeof(RtmGetTokenResponse));
                var response = serializer.Deserialize(reader) as RtmGetTokenResponse;
                if (null == response)
                    return;

                _token = response.Authorization.Token;
            }
            catch (Exception)
            {
                //TODO: Error handling
                throw;
            }
            finally
            {
                callback(_token);
            }
        }

        private void GetTimelineRequestComplete(object sender, DownloadStringCompletedEventArgs e,
            GetTimelineDelegate callback)
        {
            try
            {
                if (e.Error != null)
                    return;

                var responseXml = XElement.Parse(e.Result);
                var reader = new StringReader(responseXml.ToString());

                var serializer = new XmlSerializer(typeof(RtmCreateTimelineResponse));
                var response = serializer.Deserialize(reader) as RtmCreateTimelineResponse;
                if (null == response)
                    return;

                _timeline = response.Timeline;
            }
            catch (Exception)
            {
                //TODO: Error handling
                throw;
            }
            finally
            {
                callback(_timeline);
            }
        }

        private void GetAuthorizationRequestComplete(object sender, DownloadStringCompletedEventArgs e,
            GetAuthorizationDelegate callback)
        {
            RtmAuthorization authorization = null;

            try
            {
                if (e.Error != null)
                    return;

                var responseXml = XElement.Parse(e.Result);
                var reader = new StringReader(responseXml.ToString());

                var serializer = new XmlSerializer(typeof(RtmGetTokenResponse));
                var response = serializer.Deserialize(reader) as RtmGetTokenResponse;
                if (null == response)
                    return;

                if (!response.Status.ToLower().Equals("ok"))
                    return;

                var permissions = RtmPermissions.none;
                if (Enum.IsDefined(typeof(RtmPermissions), response.Authorization.Permissions))
                    permissions = (RtmPermissions)Enum.Parse(typeof(RtmPermissions), response.Authorization.Permissions, true);

                if (RtmPermissions.none == permissions)
                    return;

                authorization = new RtmAuthorization {
                    Token = response.Authorization.Token,
                    Permissions = permissions,
                    User = new RtmUser {
                        Id = response.Authorization.User.Id,
                        UserName = response.Authorization.User.UserName,
                        FullName = response.Authorization.User.FullName
                    }
                };
            }
            catch (Exception)
            {
                //TODO: Error handling
                throw;
            }
            finally
            {
                callback(authorization);
            }
        }

        private void GetRequest(string url, DownloadStringCompletedEventHandler callback)
        {
            var client = new WebClient();
            client.DownloadStringCompleted += callback;
            client.DownloadStringAsync(new Uri(url));
        }

        #endregion Private Methods

        #endregion Methods
    }
}
