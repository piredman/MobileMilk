using System;
using System.Collections.Generic;

namespace MobileMilk.Store.Location
{
    public class NullLocationStore : ILocationStore
    {
        public DateTime? LastSyncDate { get; set; }

        public List<Model.Location> GetAllLocations()
        {
            return new List<Model.Location>();
        }

        public void SaveLocations(IEnumerable<Model.Location> locations) { }

        public Model.Location GetLocation(Model.Location location)
        {
            return new Model.Location();
        }

        public void SaveLocation(Model.Location location) { }

        public void DeleteLocation(Model.Location location) { }

        public void SaveStore() {}
    }
}
