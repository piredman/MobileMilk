using System;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public class NullListStore : IListStore
    {
        public DateTime? LastSyncDate { get; set; }

        public List<List> GetAllLists()
        {
            return new List<List>();
        }

        public void SaveLists(IEnumerable<List> tasks) {}

        public List GetList(List task)
        {
            return new List();
        }

        public void SaveList(List task) {}

        public void DeleteList(List task) {}

        public void SaveStore() {}
    }
}
