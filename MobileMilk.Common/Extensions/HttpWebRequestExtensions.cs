using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Phone.Reactive;

namespace MobileMilk.Common.Extensions
{
    public static class HttpWebRequestExtensions
    {
        public static IObservable<T> GetRest<T>(this HttpWebRequest request)
        {
            request.Method = "GET";
            request.Accept = "application/xml";

            return Observable
                .FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                .Select(
                    response => {
                        using (var responseStream = response.GetResponseStream()) {
                            var streamReader = new StreamReader(responseStream);
                            var rawXml = streamReader.ReadToEnd();

                            var stringReader = new StringReader(rawXml);
                            var xmlReader = XmlReader.Create(stringReader);

                            var serializer = new XmlSerializer(typeof(T));
                            return (T)serializer.Deserialize(xmlReader);
                        }
                    }
                );
        }

        public static IObservable<Unit> PostXml<T>(this HttpWebRequest request, T obj)
        {
            request.Method = "POST";
            request.ContentType = "application/xml";

            return Observable
                .FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)()
                .SelectMany(
                    requestStream => {
                        using (requestStream) {
                            var serializer = new XmlSerializer(typeof(T));
                            serializer.Serialize(requestStream, obj);
                            requestStream.Close();
                        }

                        return Observable.FromAsyncPattern<WebResponse>(
                            request.BeginGetResponse,
                            request.EndGetResponse)();
                    },
                    (requestStream, webResponse) => new Unit()
                );
        }

        public static IObservable<T> GetJson<T>(this HttpWebRequest request)
        {
            request.Method = "GET";
            request.Accept = "application/json";

            return
                Observable
                    .FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                    .Select(
                        response => {
                            using (var responseStream = response.GetResponseStream())
                            {
                                var serializer = new DataContractJsonSerializer(typeof(T));
                                return (T)serializer.ReadObject(responseStream);
                            }
                        });
        }

        public static IObservable<Unit> PostJson<T>(this HttpWebRequest request, T obj)
        {
            request.Method = "POST";
            request.ContentType = "application/json";

            return
                Observable
                    .FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)()
                    .SelectMany(
                        requestStream => {
                            using (requestStream)
                            {
                                var serializer = new DataContractJsonSerializer(typeof(T));
                                serializer.WriteObject(requestStream, obj);
                                requestStream.Close();
                            }

                            return
                                Observable.FromAsyncPattern<WebResponse>(
                                    request.BeginGetResponse,
                                    request.EndGetResponse)();
                        },
                        (requestStream, webResponse) => new Unit());
        }
    }
}
