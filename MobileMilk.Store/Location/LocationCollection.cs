using System;
using System.Collections.Generic;

namespace MobileMilk.Store.Location
{
    public class LocationCollection : List<Model.Location>
    {
        public LocationCollection()
        {
            this.LastSyncDate = null;
        }
        
        public DateTime? LastSyncDate { get; set; }
    }
}
