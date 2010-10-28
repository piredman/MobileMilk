using System;
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.Store
{
    public class TaskList : List<Task>
    {
        public TaskList()
        {
            this.LastSyncDate = null;
        }
        
        public DateTime? LastSyncDate { get; set; }
    }
}
