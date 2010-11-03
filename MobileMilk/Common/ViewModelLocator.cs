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

        public TaskCollectionsViewModel TaskCollectionsViewModel
        {
            get { return this.containerLocator.Container.Resolve<TaskCollectionsViewModel>(); }
        }

        public AppSettingsViewModel AppSettingsViewModel
        {
            get { return this.containerLocator.Container.Resolve<AppSettingsViewModel>(); }
        }

        public AuthorizeViewModel AuthorizeViewModel
        {
            get { return this.containerLocator.Container.Resolve<AuthorizeViewModel>(); }
        }
        
        public TaskGroupsViewModel TaskGroupsViewModel
        {
            get
            {
                return new TaskGroupsViewModel(
                    this.TaskCollectionsViewModel.SelectedCollectionName, 
                    this.TaskCollectionsViewModel.SelectedTaskGroups,
                    this.containerLocator.Container.Resolve<INavigationService>());
            }
        }

        public TaskGroupViewModel TaskGroupViewModel
        {
            get { return this.TaskGroupsViewModel.Selected; }
        }

        public TaskViewModel TaskViewModel
        {
            get { return this.TaskGroupViewModel.SelectedTaskViewModel; }
        }

        #endregion Properties
    }
}
