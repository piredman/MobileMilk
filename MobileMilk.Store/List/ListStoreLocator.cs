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

namespace MobileMilk.Store
{
    public class ListStoreLocator : IListStoreLocator
    {
        private readonly ISettingsStore settingsStore;
        private readonly Func<string, IListStore> listStoreFactory;
        private string username;
        private IListStore listStore;

        public ListStoreLocator(ISettingsStore settingsStore, Func<string, IListStore> listStoreFactory)
        {
            this.settingsStore = settingsStore;
            this.listStoreFactory = listStoreFactory;
        }

        public IListStore GetStore()
        {
            if (string.IsNullOrEmpty(this.settingsStore.UserName))
            {
                return new NullListStore();
            }

            if (this.settingsStore.UserName != this.username)
            {
                this.username = this.settingsStore.UserName;
                var storeName = string.Format("{0}.list.store", this.username);
                this.listStore = this.listStoreFactory.Invoke(storeName);
            }

            return this.listStore;
        }
    }
}
