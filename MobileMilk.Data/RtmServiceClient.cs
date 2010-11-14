using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Reactive;
using MobileMilk.Common.Extensions;
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
                Common.Constants.ApiKey, Common.Constants.SharedSecret);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetFrobResponse>()
                .Select(ToAuthorizationUrl);
        }

        public IObservable<Authorization> GetAuthorizationToken()
        {
            var url = RtmRequestBuilder.GetTokenRequest(
                Common.Constants.ApiKey, Common.Constants.SharedSecret, _settingsStore.AuthorizationFrob);

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
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmCreateTimelineResponse>()
                .Select(ToTimeline);
        }

        #endregion Timelines

        #region Lists

        public IObservable<List<List>> GetLists()
        {
            var url = RtmRequestBuilder.GetListsRequest(
                Common.Constants.ApiKey, Common.Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetListsResponse>()
                .Select(ToLists);
        }

        #endregion Lists

        #region Locations

        public IObservable<List<Location>> GetLocations()
        {
            var url = RtmRequestBuilder.GetLocationsRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetLocationsResponse>()
                .Select(ToLocations);
        }

        #endregion Locations

        #region Tasks

        public IObservable<List<Task>> GetTasks()
        {
            var url = RtmRequestBuilder.GetTasksRequest(
                Common.Constants.ApiKey, Common.Constants.SharedSecret, _settingsStore.AuthorizationToken);

            return HttpClient
                .RequestTo(url)
                .GetRest<RtmGetTasksResponse>()
                .Select(ToTaskList);
        }

        public IObservable<Task> CompleteTask(Task task)
        {
            var url = RtmRequestBuilder.GetCompleteTaskRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken, _timeline, task);

            //TODO: The task completes correctly in RTM but the response does not seem to be caught!
            return HttpClient
                .RequestTo(url)
                .GetRest<RtmUpdateTaskResponse>()
                .Select(ToTask);
        }

        public IObservable<Task> PostponeTask(Task task)
        {
            var url = RtmRequestBuilder.GetPostponeTaskRequest(
                Constants.ApiKey, Constants.SharedSecret, _settingsStore.AuthorizationToken, _timeline, task);

            //TODO: The task completes correctly in RTM but the response does not seem to be caught!
            return HttpClient
                .RequestTo(url)
                .GetRest<RtmUpdateTaskResponse>()
                .Select(ToTask);
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
                Common.Constants.ApiKey, Common.Constants.SharedSecret, _settingsStore.AuthorizationFrob, Permissions.delete);
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

            this._timeline = response.Timeline;
            return this._timeline;
        }

        #endregion

        #region Lists

        private List<List> ToLists(RtmGetListsResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            return response.Lists.Select(list => new List {
                Id = list.Id, 
                Name = list.Name, 
                Deleted = list.Deleted.AsBool(false), 
                Archived = list.Archived.AsBool(false), 
                Locked = list.Locked.AsBool(false), 
                Position = list.Position.AsInt(-1), 
                Smart = list.Smart.AsBool(false), 
                Filter = list.Filter ?? string.Empty
            }).ToList();
        }

        #endregion Lists

        #region Locations

        private List<Location> ToLocations(RtmGetLocationsResponse response)
        {
            //TODO: handle response failure
            if (!response.Status.ToLower().Equals("ok"))
                return null;

            return response.Locations.Select(location => new Location {
                Id = location.Id, 
                Name = location.Name, 
                Address = location.Address, 
                Latitude = location.Latitude.AsDecimal(0), 
                Longitude = location.Longitude.AsDecimal(0), 
                Viewable = location.Viewable.AsBool(false), 
                Zoom = location.Zoom.AsInt(0)
            }).ToList();
        }

        #endregion Locations

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
                        notes.AddRange(series.Notes.Select(note => new Note {
                            Id = note.Id,
                            Text = note.Text,
                            Title = note.Title,
                            Created = note.Created.AsNullableDateTime(null),
                            Modified = note.Modified.AsNullableDateTime(null)
                        }));
                    }

                    var rootList = list;
                    var rootSeries = series;
                    taskList.AddRange(series.Tasks.Select(task => new Task {
                        ListId = rootList.Id,
                        TaskSeriesId = rootSeries.Id, 
                        Created = rootSeries.Created.AsNullableDateTime(null), 
                        Modified = rootSeries.Modified.AsNullableDateTime(null), 
                        Name = rootSeries.Name, 
                        Source = rootSeries.Source, 
                        Url = rootSeries.Url, 
                        LocationId = rootSeries.LocationId, 
                        Tags = tags, 
                        Participants = participants, 
                        Notes = notes, 
                        Id = task.Id, 
                        Due = task.Due.AsNullableDateTime(null), 
                        HasDueTime = task.HasDueTime.AsBool(false), 
                        Added = task.Added.AsNullableDateTime(null), 
                        Completed = task.Completed.AsNullableDateTime(null), 
                        Deleted = task.Deleted.AsNullableDateTime(null), 
                        Priority = task.Priority.AsInt(0), 
                        Postponed = task.Postponed.AsInt(0), 
                        Estimate = task.Estimate.AsNullableDateTime(null),
                        IsRepeating = rootSeries.Tasks.Count > 0
                    }));
                }
            }

            return taskList;
        }

        private Task ToTask(RtmUpdateTaskResponse response)
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
                    var tags = new List<string>();
                    if (null != series.Tags)
                    {
                        tags.AddRange(series.Tags);
                    }

                    var participants = new List<User>();
                    if (null != series.Participants)
                    {
                        participants.AddRange(series.Participants.Select(participant => new User {
                            Id = participant.Id,
                            UserName = participant.UserName,
                            FullName = participant.FullName
                        }));
                    }

                    var notes = new List<Note>();
                    if (null != series.Notes)
                    {
                        notes.AddRange(series.Notes.Select(note => new Note {
                            Id = note.Id,
                            Text = note.Text,
                            Title = note.Title,
                            Created = note.Created.AsNullableDateTime(null),
                            Modified = note.Modified.AsNullableDateTime(null)
                        }));
                    }

                    var rootList = list;
                    var rootSeries = series;
                    taskList.AddRange(series.Tasks.Select(task => new Task {
                        ListId = rootList.Id,
                        TaskSeriesId = rootSeries.Id,
                        Created = rootSeries.Created.AsNullableDateTime(null),
                        Modified = rootSeries.Modified.AsNullableDateTime(null),
                        Name = rootSeries.Name,
                        Source = rootSeries.Source,
                        Url = rootSeries.Url,
                        LocationId = rootSeries.LocationId,
                        Tags = tags,
                        Participants = participants,
                        Notes = notes,
                        Id = task.Id,
                        Due = task.Due.AsNullableDateTime(null),
                        HasDueTime = task.HasDueTime.AsBool(false),
                        Added = task.Added.AsNullableDateTime(null),
                        Completed = task.Completed.AsNullableDateTime(null),
                        Deleted = task.Deleted.AsNullableDateTime(null),
                        Priority = task.Priority.AsInt(0),
                        Postponed = task.Postponed.AsInt(0),
                        Estimate = task.Estimate.AsNullableDateTime(null),
                        IsRepeating = rootSeries.Tasks.Count > 0
                    }));
                }
            }

            return taskList.FirstOrDefault();
        }

        #endregion Tasks

        #endregion Private Methods
    }
}
