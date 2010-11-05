using System;
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.Store
{
    public class TaskCollection : List<Task>
    {
        public TaskCollection()
        {
            this.LastSyncDate = null;
        }
        
        public DateTime? LastSyncDate { get; set; }
    }
}
