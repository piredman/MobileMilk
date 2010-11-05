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
    public class TaskSynchronizationService : ITaskSynchronizationService
    {
        public const string GetTasksTask = "GetNewTasks";
        public const string SaveTasksTask = "SaveTasks";

        private readonly Func<IRtmServiceClient> _rtmServiceClientFactory;
        private readonly ITaskStoreLocator _taskStoreLocator;

        public TaskSynchronizationService(
                Func<IRtmServiceClient> rtmServiceClientFactory,
                ITaskStoreLocator taskStoreLocator)
        {
            this._rtmServiceClientFactory = rtmServiceClientFactory;
            this._taskStoreLocator = taskStoreLocator;
        }

        public IObservable<TaskCompletedSummary[]> StartSynchronization()
        {
            var getNewTasks = 
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

            //TODO: Save Tasks back to RTM
            var saveTasks = Observable.Return(new TaskCompletedSummary { Task = SaveTasksTask, Result = TaskSummaryResult.Success, Context = "0"});
            
            return Observable.ForkJoin(getNewTasks, saveTasks);
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
    }
}
