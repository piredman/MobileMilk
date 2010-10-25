using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using MobileMilk.Common;
using MobileMilk.Store;

namespace MobileMilk.ViewModels
{    
    public class AppSettingsViewModel : ViewModel
    {
        #region Delegate(s)

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SubmitCommand { get; set; }

        #endregion Delegate(s)

        #region Members

        private readonly ISettingsStore _settingsStore;
        private readonly InteractionRequest<Notification> _submitErrorInteractionRequest;
        
        private bool _canSubmit;
        private bool _isSyncing;

        private string _authorizationToken;
        private string _authorizationPermissions;

        private string _userId;
        private string _userName;
        private string _fullName;

        private bool _locationServiceAllowed;
        private bool _subscribeToPushNotifications;

        #endregion Members

        #region Constructor(s)

        public AppSettingsViewModel(ISettingsStore settingsStore, INavigationService navigationService)
            : base(navigationService)
        {
            this._settingsStore = settingsStore;
            this._submitErrorInteractionRequest = new InteractionRequest<Notification>();

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.SubmitCommand = new DelegateCommand(this.Submit, () => this.CanSubmit);

            this.AuthorizationToken = settingsStore.AuthorizationToken;
            this.AuthorizationPermissions = settingsStore.AuthorizationPermissionsAsString;

            this.UserId = settingsStore.UserId;
            this.UserName = settingsStore.UserName;
            this.FullName = settingsStore.FullName;

            this.LocationServiceAllowed = settingsStore.LocationServiceAllowed;
            this.SubscribeToPushNotifications = settingsStore.SubscribeToPushNotifications;

            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public string AuthorizationToken
        {
            get { return this._authorizationToken; }
            set
            {
                if (!string.Equals(value, this._authorizationToken))
                {
                    this._authorizationToken = value;
                    this.RaisePropertyChanged(() => this.AuthorizationToken);
                    this.CheckCanSubmit();
                }
            }
        }

        public string AuthorizationPermissions
        {
            get { return this._authorizationPermissions; }
            set
            {
                if (!string.Equals(value, this._authorizationPermissions))
                {
                    this._authorizationPermissions = value;
                    this.RaisePropertyChanged(() => this.AuthorizationPermissions);
                    this.CheckCanSubmit();
                }
            }
        }

        public string UserId
        {
            get { return this._userId; }
            set
            {
                if (!string.Equals(value, this._userId))
                {
                    this._userId = value;
                    this.RaisePropertyChanged(() => this.UserId);
                    this.CheckCanSubmit();
                }
            }
        }

        public string UserName
        {
            get { return this._userName; }
            set
            {
                if (!string.Equals(value, this._userName))
                {
                    this._userName = value;
                    this.RaisePropertyChanged(() => this.UserName);
                    this.CheckCanSubmit();
                }
            }
        }

        public string FullName
        {
            get { return this._fullName; }
            set
            {
                if (!string.Equals(value, this._fullName))
                {
                    this._fullName = value;
                    this.RaisePropertyChanged(() => this.FullName);
                    this.CheckCanSubmit();
                }
            }
        }

        public bool CanSubmit
        {
            get { return this._canSubmit; }
            set
            {
                if (!value.Equals(this._canSubmit))
                {
                    this._canSubmit = value;
                    this.RaisePropertyChanged(() => this.CanSubmit);
                    this.SubmitCommand.RaiseCanExecuteChanged();
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

        public bool LocationServiceAllowed
        {
            get { return this._locationServiceAllowed; }
            set
            {
                this._locationServiceAllowed = value;
                this.RaisePropertyChanged(() => this.LocationServiceAllowed);
            }
        }

        public bool NetworkAvailable
        {
            get { return NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None; }
        }

        public IInteractionRequest SubmitErrorInteractionRequest
        {
            get { return this._submitErrorInteractionRequest; }
        }

        public bool SubscribeToPushNotifications
        {
            get { return this._subscribeToPushNotifications; }
            set
            {
                this._subscribeToPushNotifications = value;
                this.RaisePropertyChanged(() => this.SubscribeToPushNotifications);
            }
        }

        #endregion Properties

        #region Methods

        public override sealed void IsBeingActivated()
        {
            var tombstonedLocation = Tombstoning.Load<bool?>("LocationServiceAllowed");
            var tombstonedSubscribe = Tombstoning.Load<bool?>("SettingsSubscribe");
            
            if (tombstonedLocation.HasValue)
            {
                this.LocationServiceAllowed = tombstonedLocation.Value;
            }

            if (tombstonedSubscribe.HasValue)
            {
                this.SubscribeToPushNotifications = tombstonedSubscribe.Value;
            }
        }

        public override void IsBeingDeactivated()
        {
            if (this.SubscribeToPushNotifications != this._settingsStore.SubscribeToPushNotifications)
            {
                bool? saveVal = this._subscribeToPushNotifications;
                Tombstoning.Save("SettingsSubscribe", saveVal);
            }

            if (this.LocationServiceAllowed != this._settingsStore.LocationServiceAllowed)
            {
                bool? saveVal = this._locationServiceAllowed;
                Tombstoning.Save("LocationServiceAllowed", saveVal);
            }
            
            base.IsBeingDeactivated();
        }

        public void Cancel()
        {
            this.NavigationService.GoBack();
        }

        public void Submit()
        {
            this.IsSyncing = true;
            this._settingsStore.LocationServiceAllowed = this.LocationServiceAllowed;

            if (this.SubscribeToPushNotifications == this._settingsStore.SubscribeToPushNotifications)
            {
                this.IsSyncing = false;
                this.NavigationService.GoBack();
                return;
            }

            this.IsSyncing = false;
            //ObservableExtensions.Subscribe(Observable.ObserveOnDispatcher(this.registrationServiceClient
            //                                                                  .UpdateReceiveNotifications(this.SubscribeToPushNotifications)), taskSummary =>
            //                                                                  {
            //                                                                      if (taskSummary ==
            //                                                                          TaskSummaryResult.Success)
            //                                                                      {
            //                                                                          this.settingsStore.
            //                                                                              SubscribeToPushNotifications
            //                                                                              =
            //                                                                              this.
            //                                                                                  SubscribeToPushNotifications;
            //                                                                          this.IsSyncing = false;
            //                                                                          this.NavigationService.GoBack();
            //                                                                      }
            //                                                                      else
            //                                                                      {
            //                                                                          var errorText =
            //                                                                              TaskCompletedSummaryStrings
            //                                                                                  .
            //                                                                                  GetDescriptionForResult
            //                                                                                  (taskSummary);
            //                                                                          this.IsSyncing = false;
            //                                                                          this.
            //                                                                              submitErrorInteractionRequest
            //                                                                              .Raise(
            //                                                                                  new Notification
            //                                                                                  {
            //                                                                                      Title =
            //                                                                                          "Server error",
            //                                                                                      Content =
            //                                                                                          errorText
            //                                                                                  },
            //                                                                                  n => { });
            //                                                                      }
            //                                                                  },
            //                               exception =>
            //                               {
            //                                   if (exception is WebException)
            //                                   {
            //                                       var webException = exception as WebException;
            //                                       var summary = ExceptionHandling.GetSummaryFromWebException("Update notifications", webException);
            //                                       var errorText = TaskCompletedSummaryStrings.GetDescriptionForResult(summary.Result);
            //                                       this.IsSyncing = false;
            //                                       this.submitErrorInteractionRequest.Raise(
            //                                           new Notification { Title = "Server error", Content = errorText },
            //                                           n => { });
            //                                   }
            //                                   else
            //                                   {
            //                                       throw exception;
            //                                   }
            //                               });
        }

        private void CheckCanSubmit()
        {
            this.CanSubmit = !string.IsNullOrEmpty(this._authorizationToken);
        }

        #endregion Methods
    }
}
