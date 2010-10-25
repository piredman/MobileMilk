using System;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;
using MobileMilk.Data.Entities;
using MobileMilk.Store;
using System.Windows;
using System.Collections.Generic;

namespace MobileMilk.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand AppSettingsCommand { get; set; }
        public DelegateCommand VerifyAuthorizedCommand { get; set; }
                
        #endregion Delegates
        
        #region Members

        private readonly ISettingsStore _settingsStore;        
        private readonly IRtmManager _rtmManager;

        private string _authorizationToken;
        private string _permissions;
        private string _userId;
        private string _userName;
        private string _fullName;

        private List<RtmTaskSeries> _tasks;

        private bool _isSyncing;

        #endregion Members

        #region Constructor(s)

        public HomeViewModel(ISettingsStore settingsStore, INavigationService navigationService,
            IRtmManager rtmManager)
            : base(navigationService)
        {
            this._settingsStore = settingsStore;
            this._rtmManager = rtmManager;
            this._tasks = new List<RtmTaskSeries>();

            this.AppSettingsCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/AppSettingsView.xaml", UriKind.Relative)); },
                () => !this.IsSynchronizing);

            this.GetTasks();
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public string AuthorizationToken
        {
            get { return _authorizationToken; }
            set
            {
                this._authorizationToken = value;
                this.RaisePropertyChanged(() => this.AuthorizationToken);
            }
        }

        public string Permissions
        {
            get { return _permissions; }
            set
            {
                this._permissions = value;
                this.RaisePropertyChanged(() => this.Permissions);
            }
        }

        public string UserId
        {
            get { return _userId; }
            set
            {
                this._userId = value;
                this.RaisePropertyChanged(() => this.UserId);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                this._userName = value;
                this.RaisePropertyChanged(() => this.UserName);
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                this._fullName = value;
                this.RaisePropertyChanged(() => this.FullName);
            }
        }

        public List<RtmTaskSeries> Tasks
        {
            get { return _tasks; }
            set
            {
                this._tasks = value;
                this.RaisePropertyChanged(() => this.Tasks);
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

        public void CheckAuthorization()
        {
            if (string.IsNullOrEmpty(_settingsStore.AuthorizationToken))
                this.NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
            else
            {
                _rtmManager.Token = _settingsStore.AuthorizationToken;
                _rtmManager.GetAuthorization(CheckAuthorizationComplete);
            }
        }

        public void CheckAuthorizationComplete(RtmAuthorization authorization)
        {
            if (null == authorization) {
                this.NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
                return;
            }
            
            _settingsStore.AuthorizationToken = authorization.Token;
            _settingsStore.AuthorizationPermissions = authorization.Permissions;
            _settingsStore.UserId = authorization.User.Id;
            _settingsStore.UserName = authorization.User.UserName;
            _settingsStore.FullName = authorization.User.FullName;

            _rtmManager.CreateTimeline(GetTimelineComplete);
        }

        public void GetTimelineComplete(string timeline)
        {
            //TODO: Anything to do with the timeline?
        }

        public void GetTasks()
        {
            _rtmManager.Token = _settingsStore.AuthorizationToken;
            _rtmManager.GetTasksList(GetTasksListComplete);
        }

        public void GetTasksListComplete(List<RtmTaskSeries> taskSeriesList)
        {
            Tasks = taskSeriesList;
        }

        #endregion Methods
    }
}
