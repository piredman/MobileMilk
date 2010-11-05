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

namespace MobileMilk.Store
{
    public class TaskStoreLocator : ITaskStoreLocator
    {
        private readonly ISettingsStore settingsStore;
        private readonly Func<string, ITaskStore> taskStoreFactory;
        private string username;
        private ITaskStore taskStore;

        public TaskStoreLocator(ISettingsStore settingsStore, Func<string, ITaskStore> taskStoreFactory)
        {
            this.settingsStore = settingsStore;
            this.taskStoreFactory = taskStoreFactory;
        }

        public ITaskStore GetStore()
        {
            if (string.IsNullOrEmpty(this.settingsStore.UserName))
            {
                return new NullTaskStore();
            }

            if (this.settingsStore.UserName != this.username)
            {
                this.username = this.settingsStore.UserName;
                var storeName = string.Format("{0}.task.store", this.username);
                this.taskStore = this.taskStoreFactory.Invoke(storeName);
            }

            return this.taskStore;
        }
    }
}
