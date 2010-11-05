using System;

namespace MobileMilk.Service
{
    public interface IListSynchronizationService
    {
        IObservable<TaskCompletedSummary[]> StartSynchronization();
    }
}
