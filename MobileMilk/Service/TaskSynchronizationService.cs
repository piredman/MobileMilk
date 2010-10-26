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
    public class TaskSynchronizationService : ISurveysSynchronizationService
    {
        public const string GetTasksTask = "GetNewTasks";
        public const string SaveTasksTask = "SaveTasks";

        private readonly Func<IRtmServiceClient> rtmServiceClient;
        private readonly ITaskStoreLocator taskStoreLocator;

        public TaskSynchronizationService(
                Func<IRtmServiceClient> rtmServiceClient,
                ITaskStoreLocator taskStoreLocator)
        {
            this.rtmServiceClient = rtmServiceClient;
            this.taskStoreLocator = taskStoreLocator;
        }

        public IObservable<TaskCompletedSummary[]> StartSynchronization()
        {
            var surveyStore = this.taskStoreLocator.GetStore();

            //TODO: Complete sync service
            rtmServiceClient.
            var getNewTasks = Observable.Return(new TaskCompletedSummary { Task = GetTasksTask, Result = TaskSummaryResult.AccessDenied });
            var saveTasks = Observable.Return(new TaskCompletedSummary { Task = SaveTasksTask, Result = TaskSummaryResult.AccessDenied });
            
            return Observable.ForkJoin(getNewTasks, saveTasks);
        }
    }
}
