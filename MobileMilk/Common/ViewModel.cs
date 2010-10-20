﻿using System;
using System.Runtime.Serialization;
using Microsoft.Phone.Shell;
using Microsoft.Practices.Prism.ViewModel;

namespace MobileMilk.Common
{
    [DataContract]
    public abstract class ViewModel : NotificationObject, IDisposable
    {
        private readonly INavigationService navigationService;
        private bool disposed;

        protected ViewModel(INavigationService navigationService)
        {
            PhoneApplicationService.Current.Deactivated += this.OnDeactivated;
            PhoneApplicationService.Current.Activated += this.OnActivated;

            this.navigationService = navigationService;
        }

        ~ViewModel()
        {
            this.Dispose();
        }

        public INavigationService NavigationService
        {
            get { return this.navigationService; }
        }

        public virtual void IsBeingDeactivated()
        {
        }

        public abstract void IsBeingActivated();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                PhoneApplicationService.Current.Deactivated -= this.OnDeactivated;
                PhoneApplicationService.Current.Activated -= this.OnActivated;
            }

            this.disposed = true;
        }

        private void OnDeactivated(object s, DeactivatedEventArgs e)
        {
            this.IsBeingDeactivated();
        }

        private void OnActivated(object s, ActivatedEventArgs e)
        {
            this.IsBeingActivated();
        }
    }
}
