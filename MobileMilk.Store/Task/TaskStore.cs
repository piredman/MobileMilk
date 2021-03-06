﻿using System;
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

        public TaskCollection AllTasks { get; set; }

        public DateTime? LastSyncDate
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

        public void SaveTasks(IEnumerable<Task> tasks)
        {
            //foreach (var task in tasks)
            //    task.IsNew = true;

            //foreach (var task in this.AllTasks)
            //    task.IsNew = false;

            ////Add new tasks to the list
            //this.AllTasks.AddRange(tasks.Where(
            //    newTask => !this.AllTasks.Any(task => task.TaskSeriesId == newTask.TaskSeriesId)
            //));

            //TODO: merge existing tasks

            //TODO: delete removed tasks

            //TODO: do not force update tasks all the time
            this.AllTasks.Clear();
            this.AllTasks.AddRange(tasks);

            this.SaveStore();
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
                        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TaskCollection));
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
                        this.AllTasks = new TaskCollection();
                    } 
                    else
                    {
                        var resetStore = false;
                        using (var fs = new IsolatedStorageFileStream(this.storeName, FileMode.Open, filesystem))
                        {
                            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TaskCollection));
                            try
                            {
                                this.AllTasks = serializer.ReadObject(fs) as TaskCollection;
                            }
                            catch (Exception)
                            {
                                resetStore = true;
                                this.AllTasks = new TaskCollection();
                            }
                        }

                        if (resetStore)
                            LastSyncDate = null;
                    }
                }
            }
        }

        #endregion Methods
    }
}
