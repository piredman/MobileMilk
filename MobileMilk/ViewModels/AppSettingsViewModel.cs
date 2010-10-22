﻿using System.Net;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Observable = Microsoft.Phone.Reactive.Observable;
using ObservableExtensions = Microsoft.Phone.Reactive.ObservableExtensions;
using MobileMilk.Common;
using MobileMilk.Store;
using MobileMilk.Service;

namespace MobileMilk.ViewModels
{    
    public class AppSettingsViewModel : ViewModel
    {
        #region Delegate(s)

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SubmitCommand { get; set; }

        #endregion Delegate(s)

        #region Members

        private readonly ISettingsStore settingsStore;
        private readonly InteractionRequest<Notification> submitErrorInteractionRequest;
        
        private bool canSubmit;
        private bool isSyncing;

        private string userName;
        private string password;
        private bool locationServiceAllowed;
        private bool subscribeToPushNotifications;

        #endregion Members

        #region Constructor(s)

        public AppSettingsViewModel(ISettingsStore settingsStore, INavigationService navigationService)
            : base(navigationService)
        {
            this.settingsStore = settingsStore;
            this.submitErrorInteractionRequest = new InteractionRequest<Notification>();

            this.CancelCommand = new DelegateCommand(this.Cancel);
            this.SubmitCommand = new DelegateCommand(this.Submit, () => this.CanSubmit);

            this.UserName = settingsStore.UserName;
            this.Password = settingsStore.Password;
            this.LocationServiceAllowed = settingsStore.LocationServiceAllowed;
            this.SubscribeToPushNotifications = settingsStore.SubscribeToPushNotifications;

            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public bool CanSubmit
        {
            get { return this.canSubmit; }
            set
            {
                if (!value.Equals(this.canSubmit))
                {
                    this.canSubmit = value;
                    this.RaisePropertyChanged(() => this.CanSubmit);
                    this.SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        public bool IsSyncing
        {
            get { return this.isSyncing; }
            set
            {
                this.isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSyncing);
            }
        }

        public bool LocationServiceAllowed
        {
            get { return this.locationServiceAllowed; }
            set
            {
                this.locationServiceAllowed = value;
                this.RaisePropertyChanged(() => this.LocationServiceAllowed);
            }
        }

        public bool NetworkAvailable
        {
            get { return NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None; }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (!string.Equals(value, this.password))
                {
                    this.password = value;
                    this.RaisePropertyChanged(() => this.Password);
                    this.CheckCanSubmit();
                }
            }
        }

        public IInteractionRequest SubmitErrorInteractionRequest
        {
            get { return this.submitErrorInteractionRequest; }
        }

        public bool SubscribeToPushNotifications
        {
            get { return this.subscribeToPushNotifications; }
            set
            {
                this.subscribeToPushNotifications = value;
                this.RaisePropertyChanged(() => this.SubscribeToPushNotifications);
            }
        }

        public string UserName
        {
            get { return this.userName; }
            set
            {
                if (!string.Equals(value, this.userName))
                {
                    this.userName = value;
                    this.RaisePropertyChanged(() => this.UserName);
                    this.CheckCanSubmit();
                }
            }
        }

        #endregion Properties

        #region Methods

        public override sealed void IsBeingActivated()
        {
            var tombstonedUsername = Tombstoning.Load<string>("SettingsUsername");
            var tombstonedPassword = Tombstoning.Load<string>("SettingsPassword");
            var tombstonedLocation = Tombstoning.Load<bool?>("LocationServiceAllowed");
            var tombstonedSubscribe = Tombstoning.Load<bool?>("SettingsSubscribe");

            if (tombstonedUsername != null)
            {
                this.UserName = tombstonedUsername;
            }

            if (tombstonedPassword != null)
            {
                this.Password = tombstonedPassword;
            }

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
            if (this.SubscribeToPushNotifications != this.settingsStore.SubscribeToPushNotifications)
            {
                bool? saveVal = this.subscribeToPushNotifications;
                Tombstoning.Save("SettingsSubscribe", saveVal);
            }

            if (this.LocationServiceAllowed != this.settingsStore.LocationServiceAllowed)
            {
                bool? saveVal = this.locationServiceAllowed;
                Tombstoning.Save("LocationServiceAllowed", saveVal);
            }

            Tombstoning.Save("SettingsUsername", this.UserName);
            Tombstoning.Save("SettingsPassword", this.Password);

            base.IsBeingDeactivated();
        }

        public void Cancel()
        {
            this.NavigationService.GoBack();
        }

        public void Submit()
        {
            this.IsSyncing = true;
            this.settingsStore.UserName = this.UserName;
            this.settingsStore.Password = this.Password;
            this.settingsStore.LocationServiceAllowed = this.LocationServiceAllowed;

            if (this.SubscribeToPushNotifications == this.settingsStore.SubscribeToPushNotifications)
            {
                this.IsSyncing = false;
                this.NavigationService.GoBack();
                return;
            }

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
            this.CanSubmit = !string.IsNullOrEmpty(this.userName) && !string.IsNullOrEmpty(this.password);
        }

        #endregion Methods
    }
}
