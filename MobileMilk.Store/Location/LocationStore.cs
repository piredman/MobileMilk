using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace MobileMilk.Store.Location
{
    public class LocationStore : ILocationStore
    {
        #region Members

        private readonly string storeName;

        #endregion Members

        public LocationStore(string storeName)
        {
            this.storeName = storeName;
            this.Initialize();
        }

        #region Properies

        public LocationCollection AllLocations { get; set; }

        public DateTime? LastSyncDate
        {
            get { return this.AllLocations.LastSyncDate; }
            set
            {
                this.AllLocations.LastSyncDate = value;
                this.SaveStore();
            }
        }

        #endregion Properties

        #region Methods

        public List<Model.Location> GetAllLocations()
        {
            return this.AllLocations;
        }

        public void SaveLocations(IEnumerable<Model.Location> locations)
        {
            //foreach (var location in locations)
            //    location.IsNew = true;

            //foreach (var location in this.AllLocations)
            //    location.IsNew = false;

            ////Add new locations to the location
            //this.AllLocations.AddRange(locations.Where(
            //    newLocation => !this.AllLocations.Any(location => location.LocationSeriesId == newLocation.LocationSeriesId)
            //));

            //TODO: merge existing locations

            //TODO: delete removed locations

            //TODO: do not force update locations all the time
            this.AllLocations.Clear();
            this.AllLocations.AddRange(locations);

            this.SaveStore();
        }

        public Model.Location GetLocation(Model.Location location)
        {
            return this.AllLocations.Where(a => location.Id == a.Id).FirstOrDefault();
        }

        public void SaveLocation(Model.Location location)
        {
            if (!this.AllLocations.Contains(location))
            {
                this.AllLocations.Add(location);
            }

            this.SaveStore();
        }

        public void DeleteLocation(Model.Location location)
        {
            var locationToDelete = this.GetLocation(location);
            this.AllLocations.Remove(locationToDelete);

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
                        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(LocationCollection));
                        serializer.WriteObject(fs, this.AllLocations);
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
                        this.AllLocations = new LocationCollection();
                    } 
                    else
                    {
                        var resetStore = false;
                        using (var fs = new IsolatedStorageFileStream(this.storeName, FileMode.Open, filesystem))
                        {
                            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(LocationCollection));
                            try
                            {
                                this.AllLocations = serializer.ReadObject(fs) as LocationCollection;
                            }
                            catch (Exception)
                            {
                                resetStore = true;
                                this.AllLocations = new LocationCollection();
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
