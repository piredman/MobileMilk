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
    public class ListSynchronizationService : IListSynchronizationService
    {
        public const string GetListsTask = "GetNewLists";
        public const string SaveListsTask = "SaveLists";

        private readonly Func<IRtmServiceClient> _rtmServiceClientFactory;
        private readonly IListStoreLocator _listStoreLocator;

        public ListSynchronizationService(
                Func<IRtmServiceClient> rtmServiceClientFactory,
                IListStoreLocator listStoreLocator)
        {
            this._rtmServiceClientFactory = rtmServiceClientFactory;
            this._listStoreLocator = listStoreLocator;
        }

        public IObservable<TaskCompletedSummary[]> StartSynchronization()
        {
            var getNewLists = 
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

            //TODO: Save Lists back to RTM
            var saveLists = Observable.Return(new TaskCompletedSummary { Task = SaveListsTask, Result = TaskSummaryResult.Success, Context = "0"});
            
            return Observable.ForkJoin(getNewLists, saveLists);
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
    }
}
