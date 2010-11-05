using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public class NullTaskStore : ITaskStore
    {
        public DateTime? LastSyncDate { get; set; }

        public List<Model.Task> GetAllTasks()
        {
            return new List<Task>();
        }

        public void SaveTasks(IEnumerable<Task> tasks) {}

        public Task GetTask(Model.Task task)
        {
            return new Task();
        }

        public void SaveTask(Model.Task task) {}

        public void DeleteTask(Model.Task task) {}

        public void SaveStore() {}
    }
}
