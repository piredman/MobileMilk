using System;
using System.Linq;
using System.Net;
using Microsoft.Phone.Reactive;
using MobileMilk.Store;
using MobileMilk.Data;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Service
{
    public class SynchronizationService : ISynchronizationService
    {
        #region Members

        public const string GetTasksTask = "GetNewTasks";
        public const string SaveTasksTask = "SaveTasks";
        public const string CompleteTaskTask = "CompleteTask";

        public const string GetListsTask = "GetNewLists";
        public const string SaveListsTask = "SaveLists";

        public const string GetLocationsTask = "GetNewLocations";
        public const string SaveLocationsTask = "SaveLocations";

        private readonly Func<IRtmServiceClient> _rtmServiceClientFactory;
        private readonly ITaskStoreLocator _taskStoreLocator;
        private readonly IListStoreLocator _listStoreLocator;
        private readonly ILocationStoreLocator _locationStoreLocator;

        #endregion Members

        #region Constructor(s)

        public SynchronizationService(
                Func<IRtmServiceClient> rtmServiceClientFactory,
                ITaskStoreLocator taskStoreLocator,
                IListStoreLocator listStoreLocator,
                ILocationStoreLocator locationStoreLocator)
        {
            this._rtmServiceClientFactory = rtmServiceClientFactory;
            this._taskStoreLocator = taskStoreLocator;
            this._listStoreLocator = listStoreLocator;
            this._locationStoreLocator = locationStoreLocator;
        }

        #endregion Constructor(s)

        #region Methods 

        public IObservable<TaskCompletedSummary[]> StartSynchronization()
        {
            var getTasks = GetTasks();
            var getLists = GetLists();
            var getLocations = GetLocations();               
            
            return Observable.ForkJoin(getTasks, getLists, getLocations);
        }

        public IObservable<TaskCompletedSummary> CompleteTask(Task task)
        {
            return this._rtmServiceClientFactory()
                .CompleteTask(task)
                .Select(commitedTask => {
                    var taskStore = this._taskStoreLocator.GetStore();
                    taskStore.SaveTask(commitedTask);

                    return new TaskCompletedSummary {
                        Task = CompleteTaskTask,
                        Result = TaskSummaryResult.Success,
                        Context = commitedTask.Id
                    };
                })
                .Catch((Exception exception) => {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        var summary = ExceptionHandling.GetSummaryFromWebException(GetTasksTask, webException);
                        return Observable.Return(summary);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return(new TaskCompletedSummary { Task = GetTasksTask, Result = TaskSummaryResult.AccessDenied });
                    }

                    throw exception;
                });
        }

        public IObservable<TaskCompletedSummary> PostponeTask(Task task)
        {
            return this._rtmServiceClientFactory()
                .PostponeTask(task)
                .Select(commitedTask => {
                    var taskStore = this._taskStoreLocator.GetStore();
                    taskStore.SaveTask(commitedTask);

                    return new TaskCompletedSummary {
                        Task = CompleteTaskTask,
                        Result = TaskSummaryResult.Success,
                        Context = commitedTask.Id
                    };
                })
                .Catch((Exception exception) => {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        var summary = ExceptionHandling.GetSummaryFromWebException(GetTasksTask, webException);
                        return Observable.Return(summary);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return(new TaskCompletedSummary { Task = GetTasksTask, Result = TaskSummaryResult.AccessDenied });
                    }

                    throw exception;
                });
        }

        #endregion Methods

        #region Private Methods

        private IObservable<TaskCompletedSummary> GetTasks()
        {
            return this._rtmServiceClientFactory()
                .GetTasks()
                .Select(
                    tasks => {
                        var taskStore = this._taskStoreLocator.GetStore();

                        taskStore.SaveTasks(tasks);
                        if (tasks.Count() > 0)
                            taskStore.LastSyncDate = tasks.Max(task => task.Created ?? DateTime.MinValue);

                        return new TaskCompletedSummary {
                            Task = GetTasksTask,
                            Result = TaskSummaryResult.Success,
                            Context = tasks.Count().ToString()
                        };
                    })
                .Catch((Exception exception) => {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        var summary = ExceptionHandling.GetSummaryFromWebException(GetTasksTask, webException);
                        return Observable.Return(summary);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return(new TaskCompletedSummary { Task = GetTasksTask, Result = TaskSummaryResult.AccessDenied });
                    }

                    throw exception;
                });
        }

        private IObservable<TaskCompletedSummary> GetLists()
        {
            return this._rtmServiceClientFactory()
                .GetLists()
                .Select(
                    lists => {
                        var listStore = this._listStoreLocator.GetStore();

                        listStore.SaveLists(lists);
                        if (lists.Count() > 0)
                            listStore.LastSyncDate = DateTime.Now;

                        return new TaskCompletedSummary {
                            Task = GetListsTask,
                            Result = TaskSummaryResult.Success,
                            Context = lists.Count().ToString()
                        };
                    })
                .Catch((Exception exception) => {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        var summary = ExceptionHandling.GetSummaryFromWebException(GetListsTask, webException);
                        return Observable.Return(summary);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return(new TaskCompletedSummary { Task = GetListsTask, Result = TaskSummaryResult.AccessDenied });
                    }

                    throw exception;
                });
        }

        private IObservable<TaskCompletedSummary> GetLocations()
        {
            return this._rtmServiceClientFactory()
                .GetLocations()
                .Select(
                    locations => {
                        var locationStore = this._locationStoreLocator.GetStore();

                        locationStore.SaveLocations(locations);
                        if (locations.Count() > 0)
                            locationStore.LastSyncDate = DateTime.Now;

                        return new TaskCompletedSummary {
                            Task = GetLocationsTask,
                            Result = TaskSummaryResult.Success,
                            Context = locations.Count().ToString()
                        };
                    })
                .Catch((Exception exception) => {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        var summary = ExceptionHandling.GetSummaryFromWebException(GetListsTask, webException);
                        return Observable.Return(summary);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return(new TaskCompletedSummary { Task = GetListsTask, Result = TaskSummaryResult.AccessDenied });
                    }

                    throw exception;
                });
        }

        #endregion Private Methods
    }
}
