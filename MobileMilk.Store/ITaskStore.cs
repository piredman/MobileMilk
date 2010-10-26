using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public interface ITaskStore
    {
        string LastSyncDate { get; set; }
        
        List<Task> GetAllTasks();
        
        Task GetTask(Task task);
        void SaveTask(Task task);
        void DeleteTask(Task task);

        void SaveStore();
    }
}
