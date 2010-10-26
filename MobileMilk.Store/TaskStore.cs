using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public class TaskStore : ITaskStore
    {
        #region Members

        private readonly string storeName;

        #endregion Members

        public TaskStore(string storeName)
        {
            this.storeName = storeName;
            this.Initialize();
        }

        #region Properies

        public TaskList AllTasks { get; set; }

        public string LastSyncDate
        {
            get { return this.AllTasks.LastSyncDate; }
            set
            {
                this.AllTasks.LastSyncDate = value;
                this.SaveStore();
            }
        }

        #endregion Properties

        #region Methods

        public List<Task> GetAllTasks()
        {
            return this.AllTasks;
        }

        public Task GetTask(Task task)
        {
            return this.AllTasks.Where(a => task.Id == a.Id).FirstOrDefault();
        }

        public void SaveTask(Task task)
        {
            if (!this.AllTasks.Contains(task))
            {
                this.AllTasks.Add(task);
            }

            this.SaveStore();
        }

        public void DeleteTask(Task task)
        {
            var taskToDelete = this.GetTask(task);
            this.AllTasks.Remove(taskToDelete);

            this.SaveStore();
        }

        public void SaveStore()
        {
            lock (this)
            {
                using (var filesystem = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var fs = new IsolatedStorageFileStream(this.storeName, FileMode.Create, filesystem))
                    {
                        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TaskList));
                        serializer.WriteObject(fs, this.AllTasks);
                    }
                }
            }
        }

        private void Initialize()
        {
            lock (this)
            {
                using (var filesystem = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!filesystem.FileExists(this.storeName))
                    {
                        this.AllTasks = new TaskList();
                    } 
                    else
                    {
                        using (var fs = new IsolatedStorageFileStream(this.storeName, FileMode.Open, filesystem))
                        {
                            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TaskList));
                            this.AllTasks = serializer.ReadObject(fs) as TaskList;
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}
