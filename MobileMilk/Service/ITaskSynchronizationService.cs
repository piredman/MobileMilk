using System;

namespace MobileMilk.Service
{
    public interface ISurveysSynchronizationService
    {
        IObservable<TaskCompletedSummary[]> StartSynchronization();
    }
}
