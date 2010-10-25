using System;
using System.Collections.Generic;
using System.Net;
using MobileMilk.Data.Common;
using MobileMilk.Data.Entities;

namespace MobileMilk.Data
{
    public delegate void GetFrobDelegate();
    public delegate void AuthUrlDelegate(string url);
    public delegate void TokenDelegate(string token);
    public delegate void TimelineDelegate(string timeline);

    public class RtmRequestBuilder
    {
        #region Members

        private string _apiKey;
        private string _sharedSecret;
        private string _timeline;

        private string _frob;

        #endregion Members

        #region Constructor(s)

        public RtmRequestBuilder(string apiKey, string sharedSecret)
        {
            _apiKey = apiKey;
            _sharedSecret = sharedSecret;
        }

        #endregion Constructor(s)

        #region Methods

        public string GetFrobRequest()
        {
            string frob = null;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("method", "rtm.auth.getFrob");
            return GetRequest(parameters);
        }

        public string GetAuthenticationUrl(string frob, RtmPermissions rtmPermissions)
        {
            var authParams = new Dictionary<string, string>();
            authParams.Add("api_key", _apiKey);
            authParams.Add("perms", "delete");
            authParams.Add("frob", frob);
            authParams.Add("api_sig", SignParameters(authParams));

            return CreateUrl(Constants.AuthorizationUrl, authParams);
        }

        public string GetTokenRequest(string frob)
        {
            string token = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.getToken");
            parameters.Add("frob", frob);

            return GetRequest(parameters);
        }

        public string GetCheckTokenRequest(string token)
        {
            string validatedToken = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.checkToken");

            return GetRequest(token, parameters);
        }

        public string GetTimelineRequest()
        {
            string timeline = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.timelines.create");

            return GetRequest(parameters);
        }

        #region Private Methods

        public string GetRequest(Dictionary<string, string> parameters)
        {
            return GetRequest(null, parameters);
        }

        public string GetRequest(string token, Dictionary<string, string> parameters)
        {
            parameters.Add("api_key", _apiKey);
            if (token != null) parameters.Add("auth_token", token);
            parameters.Add("api_sig", SignParameters(parameters));

            return CreateUrl(Constants.RequestUrl, parameters);
        }

        public string SignParameters(Dictionary<string, string> parameters)
        {
            string sum = String.Empty;

            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>(parameters);
            paramList.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) =>
            {
                return x.Key.CompareTo(y.Key);
            });

            sum += _sharedSecret;
            foreach (KeyValuePair<string, string> pair in paramList)
            {
                sum += pair.Key;
                sum += pair.Value;
            }

            return JeffWilcox.Utilities.Silverlight.MD5CryptoServiceProvider.GetMd5String(sum);
        }

        public string CreateUrl(string url, Dictionary<string, string> parameters)
        {
            string urlWithParams = url + "?";

            var parArray = new string[parameters.Count];
            parameters.Keys.CopyTo(parArray, 0);

            for (int i = 0; i < parArray.Length - 1; i++)
                urlWithParams += HttpUtility.UrlEncode(parArray[i]) + "=" + HttpUtility.UrlEncode(parameters[parArray[i]]) + "&";
            if (parArray.Length > 0)
                urlWithParams += parArray[parArray.Length - 1] + "=" + parameters[parArray[parArray.Length - 1]];

            return urlWithParams;
        }

        #endregion Private Methods

        #endregion Methods
    }
}
