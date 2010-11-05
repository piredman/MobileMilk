using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public class ListStore : IListStore
    {
        #region Members

        private readonly string storeName;

        #endregion Members

        public ListStore(string storeName)
        {
            this.storeName = storeName;
            this.Initialize();
        }

        #region Properies

        public ListCollection AllLists { get; set; }

        public DateTime? LastSyncDate
        {
            get { return this.AllLists.LastSyncDate; }
            set
            {
                this.AllLists.LastSyncDate = value;
                this.SaveStore();
            }
        }

        #endregion Properties

        #region Methods

        public List<List> GetAllLists()
        {
            return this.AllLists;
        }

        public void SaveLists(IEnumerable<List> lists)
        {
            //foreach (var list in lists)
            //    list.IsNew = true;

            //foreach (var list in this.AllLists)
            //    list.IsNew = false;

            ////Add new lists to the list
            //this.AllLists.AddRange(lists.Where(
            //    newList => !this.AllLists.Any(list => list.ListSeriesId == newList.ListSeriesId)
            //));

            //TODO: merge existing lists

            //TODO: delete removed lists

            //TODO: do not force update lists all the time
            this.AllLists.Clear();
            this.AllLists.AddRange(lists);

            this.SaveStore();
        }

        public List GetList(List list)
        {
            return this.AllLists.Where(a => list.Id == a.Id).FirstOrDefault();
        }

        public void SaveList(List list)
        {
            if (!this.AllLists.Contains(list))
            {
                this.AllLists.Add(list);
            }

            this.SaveStore();
        }

        public void DeleteList(List list)
        {
            var listToDelete = this.GetList(list);
            this.AllLists.Remove(listToDelete);

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
                        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ListCollection));
                        serializer.WriteObject(fs, this.AllLists);
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
                        this.AllLists = new ListCollection();
                    } 
                    else
                    {
                        var resetStore = false;
                        using (var fs = new IsolatedStorageFileStream(this.storeName, FileMode.Open, filesystem))
                        {
                            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ListCollection));
                            try
                            {
                                this.AllLists = serializer.ReadObject(fs) as ListCollection;
                            }
                            catch (Exception)
                            {
                                resetStore = true;
                                this.AllLists = new ListCollection();
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
