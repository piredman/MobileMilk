using System;
using MobileMilk.Model;

namespace MobileMilk.Service
{
    public interface ISynchronizationService
    {
        IObservable<TaskCompletedSummary[]> StartSynchronization();
        IObservable<TaskCompletedSummary> CompleteTask(Task task);
    }
}
