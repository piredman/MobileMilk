using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Phone.Reactive;
using MobileMilk.Model;
using MobileMilk.Data.Common;
using MobileMilk.Data.Messages;
using MobileMilk.Store;

namespace MobileMilk.Data
{
    public delegate void GetUrlDelegate(string url);
    public delegate void GetTokenDelegate(string token);
    public delegate void GetAuthorizationDelegate(Authorization authorization);
    public delegate void GetTimelineDelegate(string timeline);
    public delegate void GetTasksDelegate(List<Task> taskSeriesList);

    public class RtmServiceClient : IRtmServiceClient
    {
        #region Members

        private string _timeline;
        private readonly ISettingsStore _settingsStore;

        #endregion Members

        #region Properties

        public string Timeline { get { return _timeline; } set { _timeline = value; } }

        #endregion Properties

        public RtmServiceClient(ISettingsStore settingsStore)
        {
            this._settingsStore = settingsStore;
        }

        #region Methods

        #region Authorization

        public IObservable<string> GetAuthorizationUrl()
        {
            var url = RtmRequestBuilder.GetFrobRequest(
                Constants.ApiKey, Constants.SharedSecret);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetFrobResponse>()
                .Select(ToAuthorizationUrl);
        }

        public IObservable<Authorization> GetAuthorizationToken()
        {
            var url = RtmRequestBuilder.GetTokenRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationFrob);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetTokenResponse>()
                .Select(ToAuthorization);
        }

        public IObservable<Authorization> GetAuthorization()
        {
            var url = RtmRequestBuilder.GetCheckTokenRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetTokenResponse>()
                .Select(ToAuthorization);
        }

        #endregion Authorization

        #region Timelines

        public IObservable<string> CreateTimeline()
        {
            var url = RtmRequestBuilder.GetTimelineRequest(
                Constants.ApiKey, Constants.SharedSecret);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmCreateTimelineResponse>()
                .Select(ToTimeline);
        }

        #endregion Timelines

        #region Tasks

        public IObservable<List<Task>> GetTasksList()
        {
            var url = RtmRequestBuilder.GetTasksRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetTasksResponse>()
                .Select(ToTaskList);
        }

        #endregion Tasks

        #endregion Methods

        #region Private Methods

        #region Authorization

        private string ToAuthorizationUrl(RtmGetFrobResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            _settingsStore.AuthorizationFrob = response.Frob;
            return RtmRequestBuilder.GetAuthenticationUrl(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationFrob, Permissions.delete);
        }

        private Authorization ToAuthorization(RtmGetTokenResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            var permissions = Permissions.none;
            if (Enum.IsDefined(typeof(Permissions), response.Authorization.Permissions))
                permissions = (Permissions)Enum.Parse(typeof(Permissions), response.Authorization.Permissions, true);

            if (Permissions.none == permissions)
                return null;

            var authorization = new Authorization {
                Token = response.Authorization.Token,
                Permissions = permissions,
                User = new User {
                    Id = response.Authorization.User.Id,
                    UserName = response.Authorization.User.UserName,
                    FullName = response.Authorization.User.FullName
                }
            };

            _settingsStore.AuthorizationToken = authorization.Token;
            _settingsStore.AuthorizationPermissions = authorization.Permissions;
            _settingsStore.UserId = authorization.User.Id;
            _settingsStore.UserName = authorization.User.UserName;
            _settingsStore.FullName = authorization.User.FullName;

            return authorization;
        }

        #endregion Authorization

        #region Timeline

        private string ToTimeline(RtmCreateTimelineResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            return response.Timeline;
        }

        #endregion

        #region Tasks

        private List<Task> ToTaskList(RtmGetTasksResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            var taskList = new List<Task>();
            foreach (var list in response.Tasks.List)
            {
                if (null == list.TaskSeries)
                    continue;

                foreach (var series in list.TaskSeries)
                {
                    var dateCreated = Common.Convert.AsDateTime(series.Created);
                    var dateModified = Common.Convert.AsDateTime(series.Modified);

                    var tags = new List<string>();
                    if (null != series.Tags) {
                        tags.AddRange(series.Tags);
                    }

                    var participants = new List<User>();
                    if (null != series.Participants) {
                        participants.AddRange(series.Participants.Select(participant => new User {
                            Id = participant.Id, 
                            UserName = participant.UserName, 
                            FullName = participant.FullName
                        }));
                    }

                    var notes = new List<Note>();
                    if (null != series.Notes) {
                        notes.AddRange(
                            from note in series.Notes
                            let dateNoteCreated = Common.Convert.AsDateTime(note.Created)
                            let dateNoteModified = Common.Convert.AsDateTime(note.Modified)
                            select new Note {
                                Id = note.Id,
                                Title = note.Title,
                                Created = dateNoteCreated,
                                Modified = dateNoteModified
                        });
                    }

                    foreach (var task in series.Tasks)
                    {
                        var dateTaskDue = Common.Convert.AsDateTime(task.Due);
                        var hasDueTime = Common.Convert.AsBoolean(task.HasDueTime);
                        var dateTaskAdded = Common.Convert.AsDateTime(task.Added);
                        var dateTaskCompleted = Common.Convert.AsDateTime(task.Completed);
                        var dateTaskDeleted = Common.Convert.AsDateTime(task.Deleted);
                        var taskPostponed = Common.Convert.AsInt(task.Postponed);
                        var dateTaskEstimated = Common.Convert.AsDateTime(task.Estimate);

                        int priority;
                        if (!int.TryParse(task.Priority, out priority))
                            priority = 0;

                        taskList.Add(new Task {
                            TaskSeriesId = series.Id,
                            Created = (DateTime.MinValue != dateCreated) ? (DateTime?)dateCreated : null,
                            Modified = (DateTime.MinValue != dateModified) ? (DateTime?)dateModified : null,
                            Name = series.Name,
                            Source = series.Source,
                            Url = series.Url,
                            LocationId = series.LocationId,
                            Tags = tags,
                            Participants = participants,
                            Notes = notes,
                            Id = task.Id,
                            Due = (DateTime.MinValue != dateTaskDue) ? (DateTime?)dateTaskDue : null,
                            HasDueTime = hasDueTime,
                            Added = (DateTime.MinValue != dateTaskAdded) ? (DateTime?)dateTaskAdded : null,
                            Completed = (DateTime.MinValue != dateTaskCompleted) ? (DateTime?)dateTaskCompleted : null,
                            Deleted = (DateTime.MinValue != dateTaskDeleted) ? (DateTime?)dateTaskDeleted : null,
                            Priority = priority,
                            Postponed = taskPostponed,
                            Estimate = (DateTime.MinValue != dateTaskEstimated) ? (DateTime?)dateTaskEstimated : null
                        });
                    }
                }
            }

            return taskList;
        }

        #endregion Tasks

        #endregion Private Methods
    }
}
