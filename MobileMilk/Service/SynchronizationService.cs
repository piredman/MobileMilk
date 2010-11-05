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
        public const string GetTasksTask = "GetNewTasks";
        public const string SaveTasksTask = "SaveTasks";

        public const string GetListsTask = "GetNewLists";
        public const string SaveListsTask = "SaveLists";

        public const string GetLocationsTask = "GetNewLocations";
        public const string SaveLocationsTask = "SaveLocations";

        private readonly Func<IRtmServiceClient> _rtmServiceClientFactory;
        private readonly ITaskStoreLocator _taskStoreLocator;
        private readonly IListStoreLocator _listStoreLocator;
        private readonly ILocationStoreLocator _locationStoreLocator;

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

        public IObservable<TaskCompletedSummary[]> StartSynchronization()
        {
            var getTasks = 
                this._rtmServiceClientFactory()
                    .GetTasks()
                    .Select(
                        tasks => {
                            return GetTasksListCompleted(tasks);
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

            var getLists = 
                this._rtmServiceClientFactory()
                    .GetLists()
                    .Select(
                        lists => {
                            return GetListsCompleted(lists);
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

            var getLocations = 
                this._rtmServiceClientFactory()
                    .GetLocations()
                    .Select(
                        locations => {
                            return GetLocationsCompleted(locations);
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

            //TODO: Save Tasks back to RTM
            var saveTasks = Observable.Return(new TaskCompletedSummary { Task = SaveTasksTask, Result = TaskSummaryResult.Success, Context = "0"});
            
            return Observable.ForkJoin(getTasks, getLists, getLocations, saveTasks);
        }

        private TaskCompletedSummary GetTasksListCompleted(List<Task> tasks)
        {
            var taskStore = this._taskStoreLocator.GetStore();

            taskStore.SaveTasks(tasks);
            if (tasks.Count() > 0)
                taskStore.LastSyncDate = tasks.Max(task => task.Created ?? DateTime.MinValue);

            return new TaskCompletedSummary {
                Task = GetTasksTask,
                Result = TaskSummaryResult.Success,
                Context = tasks.Count().ToString()
            };
        }

        private TaskCompletedSummary GetListsCompleted(List<List> lists)
        {
            var listStore = this._listStoreLocator.GetStore();

            listStore.SaveLists(lists);
            if (lists.Count() > 0)
                listStore.LastSyncDate = DateTime.Now;

            return new TaskCompletedSummary {
                Task = GetListsTask,
                Result = TaskSummaryResult.Success,
                Context = lists.Count().ToString()
            };
        }

        private TaskCompletedSummary GetLocationsCompleted(List<Location> locations)
        {
            var locationStore = this._locationStoreLocator.GetStore();

            locationStore.SaveLocations(locations);
            if (locations.Count() > 0)
                locationStore.LastSyncDate = DateTime.Now;

            return new TaskCompletedSummary {
                Task = GetLocationsTask,
                Result = TaskSummaryResult.Success,
                Context = locations.Count().ToString()
            };
        }
    }
}
