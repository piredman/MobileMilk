using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Reactive;
using MobileMilk.Model;

namespace MobileMilk.Data
{
    public class RtmServiceClientMock : IRtmServiceClient
    {
        public IObservable<string> GetAuthorizationUrl()
        {
            var emptyUrl = new Uri("http://localhost").ToString();
            return Observable.Return(emptyUrl);
        }

        public IObservable<Authorization> GetAuthorizationToken()
        {
            var authorization = new Authorization {
                Token = MobileMilk.Common.Constants.AuthorizationToken,
                Permissions = Permissions.delete,
                User = new User {
                    Id = string.Empty,
                    UserName = string.Empty,
                    FullName = string.Empty
                }
            };

            return (IObservable<Authorization>) authorization;
        }

        public IObservable<Authorization> GetAuthorization()
        {
            throw new NotImplementedException();
        }

        public IObservable<string> CreateTimeline()
        {
            throw new NotImplementedException();
        }

        public IObservable<List<Task>> GetTasks()
        {
            throw new NotImplementedException();
        }

        public IObservable<List<List>> GetLists()
        {
            throw new NotImplementedException();
        }

        public IObservable<List<Location>> GetLocations()
        {
            throw new NotImplementedException();
        }

        public IObservable<Task> CompleteTask(Task task)
        {
            throw new NotImplementedException();
        }

        public IObservable<Task> PostponeTask(Task task)
        {
            throw new NotImplementedException();
        }
    }
}
