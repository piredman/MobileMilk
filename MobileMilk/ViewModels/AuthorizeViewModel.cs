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
        private readonly IRtmManager rtmManager;

        private string rtmAuthorizationFrob;
        private string rtmAuthorizationToken;
        private string rtmAuthorizationURL;

        private bool isSyncing;

        #endregion Members

        #region Constructor(s)
        
        public AuthorizeViewModel(ISettingsStore settingsStore, INavigationService navigationService,
            IRtmManager rtmManager)
            : base(navigationService)
        {
            this.settingsStore = settingsStore;
            this.rtmManager = rtmManager;

            this.DoneCommand = new DelegateCommand(this.Done);

            this.RtmAuthorizationFrob = settingsStore.AuthorizationFrob;
            this.RtmAuthorizationToken = settingsStore.AuthorizationToken;

            this.IsBeingActivated();
        }
                
        #endregion Constructor(s)

        #region Properties

        public string RtmAuthorizationFrob
        {
            get { return this.rtmAuthorizationFrob; }
            set
            {
                if (!string.Equals(value, this.rtmAuthorizationFrob))
                {
                    this.rtmAuthorizationFrob = value;
                    this.RaisePropertyChanged(() => this.RtmAuthorizationFrob);
                }
            }
        }

        public string RtmAuthorizationToken
        {
            get { return this.rtmAuthorizationToken; }
            set
            {
                if (!string.Equals(value, this.rtmAuthorizationToken))
                {
                    this.rtmAuthorizationToken = value;
                    this.RaisePropertyChanged(() => this.RtmAuthorizationToken);
                }
            }
        }

        public string RtmAuthorizationURL
        {
            get { return this.rtmAuthorizationURL; }
            set
            {
                if (!string.Equals(value, this.rtmAuthorizationURL))
                {
                    this.rtmAuthorizationURL = value;
                    this.RaisePropertyChanged(() => this.RtmAuthorizationURL);
                }
            }
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

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            var tombstonedFrob = Tombstoning.Load<string>("RtmAuthorizationFrob");
            var tombstonedToken = Tombstoning.Load<string>("RtmAuthorizationToken");

            if (!string.IsNullOrEmpty(tombstonedFrob))
                this.RtmAuthorizationFrob = tombstonedFrob;

            if (!string.IsNullOrEmpty(tombstonedToken))
                this.RtmAuthorizationToken = tombstonedToken;

            this.GetRtmAuthorizationPage();
        }

        public override void IsBeingDeactivated()
        {
            if (this.RtmAuthorizationFrob != settingsStore.AuthorizationFrob)
                Tombstoning.Save("RtmAuthorizationFrob", this.RtmAuthorizationFrob);

            if (this.RtmAuthorizationToken != settingsStore.AuthorizationToken)
                Tombstoning.Save("RtmAuthorizationToken", this.RtmAuthorizationToken);

            base.IsBeingDeactivated();
        }

        public void GetRtmAuthorizationPage()
        {
            rtmManager.GetAuthorizationUrl((string url) => {
                if (string.IsNullOrEmpty(url))
                    return;

                this.RtmAuthorizationURL = url;
            });
        }

        public void Done()
        {
            IsSynchronizing = true;

            rtmManager.GetAuthorizationToken((string token) => {
                if (string.IsNullOrEmpty(token))
                    return;

                this.RtmAuthorizationToken = token;

                settingsStore.AuthorizationFrob = this.RtmAuthorizationFrob;
                settingsStore.AuthorizationToken = this.RtmAuthorizationToken;

                IsSynchronizing = false;
                this.NavigationService.GoBack();
            });
        }

        #endregion Methods
    }
}
