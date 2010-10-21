using System;
using System.Windows;
using Funq;
using MobileMilk.Service;
using MobileMilk.Store;
using MobileMilk.ViewModels;

namespace MobileMilk.Common
{
    public class ContainerLocator : IDisposable
    {
        #region Members

        private bool disposed;

        #endregion Members

        #region Constructor(s)

        public ContainerLocator()
        {
            this.Container = new Container();
            this.ConfigureContainer();
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
                this.Container.Dispose();
            }

            this.disposed = true;
        }

        #endregion Constructor(s)

        #region Properties

        public Container Container { get; private set; }

        #endregion Properties

        #region Methods

        private void ConfigureContainer()
        {
            //example: this.Container.Register("ServiceUri", new Uri("http://127.0.0.1:8080/Survey/"));

            this.Container.Register<ISettingsStore>(c => new SettingsStore());
            this.Container.Register<INavigationService>(_ =>
                new ApplicationFrameNavigationService(((App)Application.Current).RootFrame));

            // LocationService:
            // 1. Registration
            this.Container.Register<ILocationService>(c => new LocationService(c.Resolve<ISettingsStore>()));

            // 2. Starts the service by trying to get the current location
            this.Container.Resolve<ILocationService>().TryToGetCurrentLocation();

            // View Models
            this.Container.Register(
                c => new HomeViewModel(c.Resolve<INavigationService>()));
            this.Container.Register(
                c => new AppSettingsViewModel(
                         c.Resolve<ISettingsStore>(),
                         c.Resolve<INavigationService>()))
                .ReusedWithin(ReuseScope.None);

            // The ONLY_PHONE symbol is only defined in the "OnlyPhone" configuration to run the phone project standalone
            #if ONLY_PHONE
                // EXAMPLE
                // this.Container.Register<ISurveysServiceClient>(c => new SurveysServiceClientMock(c.Resolve<ISettingsStore>()));
            #else
                // EXAMPLE
                // this.Container.Register<ISurveysServiceClient>(c => new SurveysServiceClient(c.ResolveNamed<Uri>("ServiceUri"), c.Resolve<ISettingsStore>()));
            #endif
        }

        #endregion Methods
    }
}
