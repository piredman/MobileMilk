using System;
using System.Collections.Generic;
using System.Net;
using MobileMilk.Data.Common;
using MobileMilk.Model;

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

        #region Authorization

        public string GetFrobRequest()
        {
            string frob = null;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("method", "rtm.auth.getFrob");
            return BuildRequest(parameters);
        }

        public string GetAuthenticationUrl(string frob, Permissions Permissions)
        {
            var authParams = new Dictionary<string, string>();
            authParams.Add("api_key", _apiKey);
            authParams.Add("perms", "delete");
            authParams.Add("frob", frob);
            authParams.Add("api_sig", SignArguments(authParams));

            return CreateUrl(Constants.AuthorizationUrl, authParams);
        }

        public string GetTokenRequest(string frob)
        {
            string token = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.getToken");
            parameters.Add("frob", frob);

            return BuildRequest(parameters);
        }

        public string GetCheckTokenRequest(string token)
        {
            string validatedToken = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.checkToken");

            return BuildRequest(token, parameters);
        }

        #endregion Authorization

        #region Timelines

        public string GetTimelineRequest()
        {
            string timeline = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.timelines.create");

            return BuildRequest(parameters);
        }

        #endregion Timelines

        #region Tasks

        public string GetTasksRequest(string token)
        {
            string timeline = null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.tasks.getList");

            return BuildRequest(token, parameters);
        }

        #endregion Tasks

        #region Private Methods

        public string BuildRequest(Dictionary<string, string> parameters)
        {
            return BuildRequest(null, parameters);
        }

        public string BuildRequest(string token, Dictionary<string, string> parameters)
        {
            parameters.Add("api_key", _apiKey);
            if (token != null) parameters.Add("auth_token", token);
            parameters.Add("api_sig", SignArguments(parameters));

            return CreateUrl(Constants.RequestUrl, parameters);
        }

        public string SignArguments(Dictionary<string, string> parameters)
        {
            string sum = String.Empty;

            var paramList = new List<KeyValuePair<string, string>>(parameters);
            paramList.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => {
                return x.Key.CompareTo(y.Key);
            });

            sum += _sharedSecret;
            foreach (KeyValuePair<string, string> pair in paramList) {
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
