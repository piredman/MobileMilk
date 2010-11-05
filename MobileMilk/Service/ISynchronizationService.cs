using System;

namespace MobileMilk.Service
{
    public interface ISynchronizationService
    {
        IObservable<TaskCompletedSummary[]> StartSynchronization();
    }
}
