using System;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;
using MobileMilk.Store;
using System.Windows.Threading;

namespace MobileMilk.ViewModels
{
    public class AuthorizeViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand DoneCommand { get; set; }
        
        #endregion Delegates
        
        #region Members

        private readonly ISettingsStore settingsStore;
        private readonly IRtmServiceClient rtmManager;

        private string _authorizationFrob;
        private string _authorizationToken;
        private string _authorizationURL;

        private bool _isSyncing;

        #endregion Members

        #region Constructor(s)
        
        public AuthorizeViewModel(ISettingsStore settingsStore, INavigationService navigationService,
            IRtmServiceClient rtmManager)
            : base(navigationService)
        {
            this.settingsStore = settingsStore;
            this.rtmManager = rtmManager;

            this.DoneCommand = new DelegateCommand(this.Done);

            this.AuthorizationFrob = settingsStore.AuthorizationFrob;
            this.AuthorizationToken = settingsStore.AuthorizationToken;

            this.IsBeingActivated();
        }
                
        #endregion Constructor(s)

        #region Properties

        public string AuthorizationFrob
        {
            get { return this._authorizationFrob; }
            set
            {
                if (!string.Equals(value, this._authorizationFrob))
                {
                    this._authorizationFrob = value;
                    this.RaisePropertyChanged(() => this.AuthorizationFrob);
                }
            }
        }

        public string AuthorizationToken
        {
            get { return this._authorizationToken; }
            set
            {
                if (!string.Equals(value, this._authorizationToken))
                {
                    this._authorizationToken = value;
                    this.RaisePropertyChanged(() => this.AuthorizationToken);
                }
            }
        }

        public string AuthorizationURL
        {
            get { return this._authorizationURL; }
            set
            {
                if (!string.Equals(value, this._authorizationURL))
                {
                    this._authorizationURL = value;
                    this.RaisePropertyChanged(() => this.AuthorizationURL);
                }
            }
        }

        public bool IsSynchronizing
        {
            get { return this._isSyncing; }
            set
            {
                this._isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSynchronizing);
            }
        }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            var tombstonedFrob = Tombstoning.Load<string>("AuthorizationFrob");
            var tombstonedToken = Tombstoning.Load<string>("AuthorizationToken");

            if (!string.IsNullOrEmpty(tombstonedFrob))
                this.AuthorizationFrob = tombstonedFrob;

            if (!string.IsNullOrEmpty(tombstonedToken))
                this.AuthorizationToken = tombstonedToken;

            this.GetAuthorizationPage();
        }

        public override void IsBeingDeactivated()
        {
            if (this.AuthorizationFrob != settingsStore.AuthorizationFrob)
                Tombstoning.Save("AuthorizationFrob", this.AuthorizationFrob);

            if (this.AuthorizationToken != settingsStore.AuthorizationToken)
                Tombstoning.Save("AuthorizationToken", this.AuthorizationToken);

            base.IsBeingDeactivated();
        }

        public void GetAuthorizationPage()
        {
            rtmManager.GetAuthorizationUrl((string url) => {
                if (string.IsNullOrEmpty(url))
                    return;

                this.AuthorizationURL = url;
            });
        }

        public void Done()
        {
            IsSynchronizing = true;

            rtmManager.GetAuthorizationToken((string token) => {
                if (string.IsNullOrEmpty(token))
                    return;

                this.AuthorizationToken = token;

                settingsStore.AuthorizationFrob = this.AuthorizationFrob;
                settingsStore.AuthorizationToken = this.AuthorizationToken;

                IsSynchronizing = false;
                this.NavigationService.GoBack();
            });
        }

        #endregion Methods
    }
}
