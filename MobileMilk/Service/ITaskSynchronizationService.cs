using System;

namespace MobileMilk.Service
{
    public interface ITaskSynchronizationService
    {
        IObservable<TaskCompletedSummary[]> StartSynchronization();
    }
}
