using System;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public interface ITaskStore
    {
        DateTime? LastSyncDate { get; set; }

        List<Task> GetAllTasks();
        void SaveTasks(IEnumerable<Task> tasks);
        
        Task GetTask(Task task);
        void SaveTask(Task task);
        void DeleteTask(Task task);

        void SaveStore();
    }
}
