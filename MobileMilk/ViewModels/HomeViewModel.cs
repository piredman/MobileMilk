using System;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;
using MobileMilk.Store;
using System.Windows;

namespace MobileMilk.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand AppSettingsCommand { get; set; }
        public DelegateCommand VerifyAuthorizedCommand { get; set; }
                
        #endregion Delegates
        
        #region Members

        private readonly ISettingsStore settingsStore;
        private readonly IRtmAuthorizationService rtmAuthorizationService;

        private bool isSyncing;

        #endregion Members

        #region Constructor(s)        

        public HomeViewModel(ISettingsStore settingsStore, INavigationService navigationService, 
            IRtmAuthorizationService rtmAuthorizationService)
            : base(navigationService)
        {
            this.settingsStore = settingsStore;
            this.rtmAuthorizationService = rtmAuthorizationService;

            this.AppSettingsCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/AppSettingsView.xaml", UriKind.Relative)); },
                () => !this.IsSynchronizing);

            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Methods

        public bool SettingAreConfigured
        {
            get { return !this.SettingAreNotConfigured; }
        }

        public bool SettingAreNotConfigured
        {
            get { return true; }
            //TODO: this
            //get { return this.surveyStoreLocator.GetStore() is NullSurveyStore; }
        }

        public override void IsBeingActivated()
        {
            //TODO: this!
            //if (this.selectedSurveyTemplate == null)
            //{
            //    var tombstoned = Tombstoning.Load<SurveyTemplateViewModel>("SelectedTemplate");
            //    if (tombstoned != null)
            //    {
            //        this.SelectedSurveyTemplate = new SurveyTemplateViewModel(tombstoned.Template, this.NavigationService);
            //    }

            //    this.selectedPivotIndex = Tombstoning.Load<int>("MainPivot");
            //}
        }

        public void NavigateIfNotAuthorized()
        {
            if (!IsAuthorized())
                this.NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
        }

        public bool IsAuthorized()
        {
            var authorized = !string.IsNullOrEmpty(settingsStore.AuthorizationToken);
            if (authorized)
                authorized = rtmAuthorizationService.CheckAuthorizationToken(settingsStore.AuthorizationToken);

            return authorized;
        }

        public bool IsSynchronizing
        {
            get { return this.isSyncing; }
            set
            {
                this.isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSynchronizing);
            }
        }

        #endregion Methods
    }
}
