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

    public static class RtmRequestBuilder
    {
        #region Methods

        #region Authorization

        public static string GetFrobRequest(string apiKey, string sharedSecret)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.getFrob");
            return BuildRequest(apiKey, sharedSecret, parameters);
        }

        public static string GetAuthenticationUrl(string apiKey, string sharedSecret, string frob, Permissions Permissions)
        {
            var authParams = new Dictionary<string, string>();
            authParams.Add("api_key", apiKey);
            authParams.Add("perms", "delete");
            authParams.Add("frob", frob);
            authParams.Add("api_sig", SignArguments(sharedSecret, authParams));
            return CreateUrl(Constants.AuthorizationUrl, authParams);
        }

        public static string GetTokenRequest(string apiKey, string sharedSecret, string frob)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.getToken");
            parameters.Add("frob", frob);
            return BuildRequest(apiKey, sharedSecret, parameters);
        }

        public static string GetCheckTokenRequest(string apiKey, string sharedSecret, string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.auth.checkToken");
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        #endregion Authorization

        #region Timelines

        public static string GetTimelineRequest(string apiKey, string sharedSecret, string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.timelines.create");
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        #endregion Timelines

        #region Lists

        public static string GetListsRequest(string apiKey, string sharedSecret, string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.lists.getList");
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        #endregion Lists

        #region Locations

        public static string GetLocationsRequest(string apiKey, string sharedSecret, string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.locations.getList");
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        #endregion Locations

        #region Tasks

        public static string GetTasksRequest(string apiKey, string sharedSecret, string token)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("method", "rtm.tasks.getList");
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        public static string GetCompleteTaskRequest(string apiKey, string sharedSecret, string token, string timeline, Task task)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("timeline", timeline);
            parameters.Add("method", "rtm.tasks.complete");
            parameters.Add("task_id", task.Id);
            parameters.Add("taskseries_id", task.TaskSeriesId);
            parameters.Add("list_id", task.ListId);
            return BuildRequest(apiKey, sharedSecret, token, parameters);
        }

        #endregion Tasks

        #region Private Methods

        public static string BuildRequest(string apiKey, string sharedSecret, Dictionary<string, string> parameters)
        {
            return BuildRequest(apiKey, sharedSecret, null, parameters);
        }

        public static string BuildRequest(string apiKey, string sharedSecret, string token, Dictionary<string, string> parameters)
        {
            parameters.Add("api_key", apiKey);
            if (token != null) parameters.Add("auth_token", token);
            parameters.Add("api_sig", SignArguments(sharedSecret, parameters));
            return CreateUrl(Constants.RequestUrl, parameters);
        }

        public static string SignArguments(string sharedSecret, Dictionary<string, string> parameters)
        {
            string sum = String.Empty;

            var paramList = new List<KeyValuePair<string, string>>(parameters);
            paramList.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => {
                return x.Key.CompareTo(y.Key);
            });

            sum += sharedSecret;
            foreach (KeyValuePair<string, string> pair in paramList) {
                sum += pair.Key;
                sum += pair.Value;
            }

            return JeffWilcox.Utilities.Silverlight.MD5CryptoServiceProvider.GetMd5String(sum);
        }

        public static string CreateUrl(string url, Dictionary<string, string> parameters)
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
