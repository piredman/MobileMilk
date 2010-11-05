using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MobileMilk.Store.Location;

namespace MobileMilk.Store
{
    public class LocationStoreLocator : ILocationStoreLocator
    {
        private readonly ISettingsStore settingsStore;
        private readonly Func<string, ILocationStore> locationStoreFactory;
        private string username;
        private ILocationStore locationStore;

        public LocationStoreLocator(ISettingsStore settingsStore, Func<string, ILocationStore> locationStoreFactory)
        {
            this.settingsStore = settingsStore;
            this.locationStoreFactory = locationStoreFactory;
        }

        public ILocationStore GetStore()
        {
            if (string.IsNullOrEmpty(this.settingsStore.UserName))
            {
                return new NullLocationStore();
            }

            if (this.settingsStore.UserName != this.username)
            {
                this.username = this.settingsStore.UserName;
                var storeName = string.Format("{0}.location.store", this.username);
                this.locationStore = this.locationStoreFactory.Invoke(storeName);
            }

            return this.locationStore;
        }
    }
}
