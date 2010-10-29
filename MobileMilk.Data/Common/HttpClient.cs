using System;
using System.Net;

namespace MobileMilk.Data.Common
{
    public static class HttpClient
    {
        public static HttpWebRequest RequestTo(string url)
        {
            return RequestTo(new Uri(url));
        }

        public static HttpWebRequest RequestTo(Uri uri)
        {
            return (HttpWebRequest)WebRequest.Create(uri);
        }
    }
}