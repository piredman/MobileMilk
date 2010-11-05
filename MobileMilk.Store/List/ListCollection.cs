using System;
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.Store
{
    public class ListCollection : List<List>
    {
        public ListCollection()
        {
            this.LastSyncDate = null;
        }
        
        public DateTime? LastSyncDate { get; set; }
    }
}
