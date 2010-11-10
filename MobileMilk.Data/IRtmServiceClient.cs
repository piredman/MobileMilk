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
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.Data
{
    public interface IRtmServiceClient
    {
        IObservable<string> GetAuthorizationUrl();
        IObservable<Authorization> GetAuthorizationToken();
        IObservable<Authorization> GetAuthorization();
        IObservable<string> CreateTimeline();

        IObservable<List<Task>> GetTasks();
        IObservable<List<List>> GetLists();
        IObservable<List<Location>> GetLocations();

        IObservable<Task> CompleteTask(Task task);
    }
}
