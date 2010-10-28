using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;
using MobileMilk.Model;
using MobileMilk.Store;
using MobileMilk.Service;
using System.Collections.Generic;
using Microsoft.Phone.Reactive;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Notification = Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification;

namespace MobileMilk.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand StartSyncCommand { get; set; }
        public DelegateCommand AppSettingsCommand { get; set; }
        public DelegateCommand VerifyAuthorizedCommand { get; set; }
                
        #endregion Delegates
        
        #region Members

        private readonly InteractionRequest<Notification> submitErrorInteractionRequest;
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        private readonly ITaskStoreLocator _taskStoreLocator;
        private readonly ITaskSynchronizationService _synchronizationService;
        private ITaskStore lastTaskStore;

        private ObservableCollection<TaskListItemViewModel> observableTaskListItems;
        private CollectionViewSource todaysTasksViewSource;
        private int selectedPivotIndex;
        private TaskListItemViewModel selectedTaskListItem;

        private readonly IRtmServiceClient _rtmServiceClient;

        private bool _isSyncing;

        #endregion Members

        #region Constructor(s)

        public HomeViewModel(
            INavigationService navigationService,
            IRtmServiceClient rtmServiceClient,
            ITaskStoreLocator taskStoreLocator,
            ITaskSynchronizationService synchronizationService)
            : base(navigationService)
        {
            this._rtmServiceClient = rtmServiceClient;
            this._taskStoreLocator = taskStoreLocator;
            this._synchronizationService = synchronizationService;

            this.submitErrorInteractionRequest = new InteractionRequest<Notification>();
            this.submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            this.StartSyncCommand = new DelegateCommand(
                () => { this.StartSync(); },
                () => !this.IsSyncing && !this.SettingAreNotConfigured);

            this.AppSettingsCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/AppSettingsView.xaml", UriKind.Relative)); },
                () => !this.IsSyncing);

            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public ICollectionView TodaysTasks
        {
            get { return this.todaysTasksViewSource.View; }
        }

        public int SelectedPivotIndex
        {
            get { return this.selectedPivotIndex; }

            set
            {
                this.selectedPivotIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskListItemViewModel SelectedTaskListItem
        {
            get { return this.selectedTaskListItem; }

            set
            {
                if (value != null)
                {
                    this.selectedTaskListItem = value;
                    this.RaisePropertyChanged(() => this.SelectedTaskListItem);
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

        public bool SettingAreConfigured
        {
            get { return !this.SettingAreNotConfigured; }
        }

        public bool SettingAreNotConfigured
        {
            get
            {
                return this._taskStoreLocator.GetStore() is NullTaskStore;
            }
        }

        public bool TasksHaveBeenSynced
        {
            get { return !this.TasksHaveNotBeenSynced; }
        }

        public bool TasksHaveNotBeenSynced
        {
            get
            {
                var store = this._taskStoreLocator.GetStore();
                var nullStore = (store is NullTaskStore);

                return ((nullStore) || (store.LastSyncDate == null));
            }
        }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            if (this.selectedTaskListItem == null)
            {
                var tombstoned = Tombstoning.Load<TaskListItemViewModel>("SelectedTemplate");
                if (tombstoned != null)
                {
                    this.SelectedTaskListItem = new TaskListItemViewModel(tombstoned.TaskItem, this.NavigationService);
                }

                this.selectedPivotIndex = Tombstoning.Load<int>("MainPivot");
            }
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTemplate", this.SelectedTaskListItem);
            Tombstoning.Save("MainPivot", this.SelectedPivotIndex);

            base.IsBeingDeactivated();
        }

        public void CheckAuthorization()
        {
            this._rtmServiceClient
                    .GetAuthorization()
                    .ObserveOnDispatcher()
                    .Subscribe(
                        authorization => {
                            CheckAuthorizationComplete(authorization);
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

        public void CheckAuthorizationComplete(Authorization authorization)
        {
            if (null == authorization) {
                this.NavigationService.Navigate(new Uri("/Views/AuthorizeView.xaml", UriKind.Relative));
                return;
            }

            this._rtmServiceClient
                    .CreateTimeline()
                    .ObserveOnDispatcher()
                    .Subscribe(
                        timeline => {
                            StartSync();
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

        public void StartSync()
        {
            if (this.IsSyncing)
                return;

            this.IsSyncing = true;
            this._synchronizationService
                    .StartSynchronization()
                    .ObserveOnDispatcher()
                    .Subscribe(taskSummaries => this.SyncCompleted(taskSummaries));
        }

        private void SyncCompleted(IEnumerable<TaskCompletedSummary> taskSummaries)
        {
            var stringBuilder = new StringBuilder();

            var errorSummary =
                taskSummaries.FirstOrDefault(
                    s =>
                        s.Result == TaskSummaryResult.UnreachableServer ||
                        s.Result == TaskSummaryResult.AccessDenied);

            if (errorSummary != null)
            {
                stringBuilder.AppendLine(GetDescriptionForSummary(errorSummary));
                this.submitErrorInteractionRequest.Raise(
                        new Notification { Title = "Synchronization error", Content = stringBuilder.ToString() },
                        n => { });
            } 
            else
            {
                foreach (var task in taskSummaries)
                {
                    stringBuilder.AppendLine(GetDescriptionForSummary(task));
                }

                if (taskSummaries.Any(t => t.Result != TaskSummaryResult.Success))
                {
                    this.submitErrorInteractionRequest.Raise(
                        new Notification { Title = "Synchronization error", Content = stringBuilder.ToString() },
                        n => { });
                } 
                else
                {
                    this.submitNotificationInteractionRequest.Raise(
                        new Notification { Title = stringBuilder.ToString(), Content = null },
                        n => { });
                }
            }

            // Update the View Model status
            this.BuildPivotDimensions();
            this.RaisePropertyChanged(string.Empty);
            this.IsSyncing = false;
            this.UpdateCommandsForSync();
        }

        public void Refresh()
        {
            if (this._taskStoreLocator.GetStore() != this.lastTaskStore)
            {
                this.lastTaskStore = this._taskStoreLocator.GetStore();
                this.BuildPivotDimensions();
                this.RaisePropertyChanged(string.Empty);
                this.StartSyncCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Methods

        #region Private Methods

        private void BuildPivotDimensions()
        {
            this.observableTaskListItems = new ObservableCollection<TaskListItemViewModel>();
            var taskListItemViewModels = this._taskStoreLocator.GetStore().GetAllTasks().Select(t => 
                    new TaskListItemViewModel(t, this.NavigationService)).ToList();
            taskListItemViewModels.ForEach(this.observableTaskListItems.Add);

            // Listen for task changes
            // TODO: listen for task changes
            //this.ListenSurveyChanges();

            // Create collection views
            this.todaysTasksViewSource = new CollectionViewSource { Source = this.observableTaskListItems };

            this.todaysTasksViewSource.Filter += (o, e) => e.Accepted = (((TaskListItemViewModel)e.Item).Due ?? DateTime.MaxValue) <= DateTime.Today;
            this.todaysTasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));

            this.todaysTasksViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedTaskListItem = (TaskListItemViewModel)this.todaysTasksViewSource.View.CurrentItem;

            // Initialize the selected survey template
            this.HandleCurrentSectionChanged();
        }

        private void HandleCurrentSectionChanged()
        {
            ICollectionView currentView = null;
            switch (this.SelectedPivotIndex)
            {
                case 0:
                    currentView = this.TodaysTasks;
                    break;
            }

            if (currentView != null)
            {
                this.SelectedTaskListItem = (TaskListItemViewModel)currentView.CurrentItem;
            }
        }

        private void UpdateCommandsForSync()
        {
            this.StartSyncCommand.RaiseCanExecuteChanged();
            this.AppSettingsCommand.RaiseCanExecuteChanged();
        }

        private static string GetDescriptionForSummary(TaskCompletedSummary summary)
        {
            switch (summary.Result)
            {
                case TaskSummaryResult.Success:
                    switch (summary.Task)
                    {
                        case TaskSynchronizationService.GetTasksTask:
                            return string.Format(
                                CultureInfo.InvariantCulture,
                                "syncronized {0} tasks from RTM",
                                summary.Context);
                        case TaskSynchronizationService.SaveTasksTask:
                            return string.Format(
                                CultureInfo.InvariantCulture,
                                "updated {0} tasks with RTM",
                                summary.Context);
                        default:
                            return string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}: {1}",
                                summary.Task,
                                TaskCompletedSummaryStrings.GetDescriptionForResult(summary.Result));
                    }
                default:
                    return TaskCompletedSummaryStrings.GetDescriptionForResult(summary.Result);
            }
        }

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
