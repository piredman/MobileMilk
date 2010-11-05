using System;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store
{
    public interface IListStore
    {
        DateTime? LastSyncDate { get; set; }

        List<List> GetAllLists();
        void SaveLists(IEnumerable<List> tasks);
        
        List GetList(List task);
        void SaveList(List task);
        void DeleteList(List task);

        void SaveStore();
    }
}
