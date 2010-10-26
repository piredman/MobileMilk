using System;
using System.Net;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;
using MobileMilk.Store;
using MobileMilk.Service;
using System.Windows.Threading;
using Microsoft.Phone.Reactive;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Notification = Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification;

namespace MobileMilk.ViewModels
{
    public class AuthorizeViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand DoneCommand { get; set; }
        
        #endregion Delegates
        
        #region Members

        private readonly InteractionRequest<Notification> submitErrorInteractionRequest;

        private readonly ISettingsStore settingsStore;
        private readonly IRtmServiceClient rtmServiceClient;

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
            this.rtmServiceClient = rtmManager;

            this.DoneCommand = new DelegateCommand(this.Done);
            this.IsBeingActivated();
        }
                
        #endregion Constructor(s)

        #region Properties

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

        public bool IsSyncing
        {
            get { return this._isSyncing; }
            set
            {
                this._isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSyncing);
            }
        }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            this.GetAuthorizationPage();
        }

        public override void IsBeingDeactivated()
        {
            base.IsBeingDeactivated();
        }

        public void GetAuthorizationPage()
        {
            this.rtmServiceClient
                .GetAuthorizationUrl()
                .ObserveOnDispatcher()
                .Subscribe(
                    url => {
                        this.AuthorizationURL = url;
                    },
                    exception => {
                        if (exception is WebException)
                            this.HandleWebException(exception as WebException, () => this.NavigationService.GoBack());
                        else if (exception is UnauthorizedAccessException)
                            this.HandleUnauthorizedException(() => this.NavigationService.GoBack());
                        else
                            throw exception;
                    });
        }

        public void Done()
        {
            IsSyncing = true;

            this.rtmServiceClient
                .GetAuthorizationToken()
                .ObserveOnDispatcher()
                .Subscribe(
                    authorization => {
                        IsSyncing = false;
                        this.NavigationService.GoBack();
                    },
                    exception => {
                        if (exception is WebException)
                            this.HandleWebException(exception as WebException, () => this.NavigationService.GoBack());
                        else if (exception is UnauthorizedAccessException)
                            this.HandleUnauthorizedException(() => this.NavigationService.GoBack());
                        else
                            throw exception;
                    });
        }

        #endregion Methods

        #region Private Methods

        private void HandleWebException(WebException webException, Action afterNotification)
        {
            var summary = ExceptionHandling.GetSummaryFromWebException(string.Empty, webException);
            var errorText = TaskCompletedSummaryStrings.GetDescriptionForResult(summary.Result);
            this.IsSyncing = false;
            this.submitErrorInteractionRequest.Raise(
                new Notification { Title = "Server error", Content = errorText },
                n => afterNotification());
        }

        private void HandleUnauthorizedException(Action afterNotification)
        {
            this.IsSyncing = false;
            this.submitErrorInteractionRequest.Raise(
                new Notification { 
                    Title = "Server error", 
                    Content = TaskCompletedSummaryStrings.GetDescriptionForResult(TaskSummaryResult.AccessDenied) 
                },
                n => afterNotification());
        }

        #endregion Private Methods
    }
}
