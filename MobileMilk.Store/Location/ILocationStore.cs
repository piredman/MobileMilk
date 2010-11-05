using System;
using System.Collections.Generic;
using MobileMilk.Model;

namespace MobileMilk.Store.Location
{
    public interface ILocationStore
    {
        DateTime? LastSyncDate { get; set; }

        List<Model.Location> GetAllLocations();
        void SaveLocations(IEnumerable<Model.Location> locations);

        Model.Location GetLocation(Model.Location location);
        void SaveLocation(Model.Location location);
        void DeleteLocation(Model.Location location);

        void SaveStore();
    }
}
