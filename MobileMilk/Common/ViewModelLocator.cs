using System;
using MobileMilk.ViewModels;

namespace MobileMilk.Common
{
    public class ViewModelLocator : IDisposable
    {
        #region Members

        private readonly ContainerLocator containerLocator;
        private bool disposed;

        #endregion Members

        #region Constructor(s)

        public ViewModelLocator()
        {
            this.containerLocator = new ContainerLocator();
        }

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
                this.containerLocator.Dispose();
            }

            this.disposed = true;
        }

        #endregion Constructor(s)

        #region Properties

        public HomeViewModel HomeViewModel
        {
            get { return this.containerLocator.Container.Resolve<HomeViewModel>(); }
        }

        public TasksByDueViewModel TasksByDueViewModel
        {
            get { return this.containerLocator.Container.Resolve<TasksByDueViewModel>(); }
        }

        public AppSettingsViewModel AppSettingsViewModel
        {
            get { return this.containerLocator.Container.Resolve<AppSettingsViewModel>(); }
        }

        public AuthorizeViewModel AuthorizeViewModel
        {
            get { return this.containerLocator.Container.Resolve<AuthorizeViewModel>(); }
        }

        #endregion Properties
    }
}
