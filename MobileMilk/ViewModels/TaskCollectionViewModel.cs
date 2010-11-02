﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Common.Extensions;
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
    public class TaskCollectionViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand StartSyncCommand { get; set; }
        public DelegateCommand AppSettingsCommand { get; set; }
                
        #endregion Delegates
        
        #region Members

        private readonly InteractionRequest<Notification> submitErrorInteractionRequest;
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        private readonly IRtmServiceClient _rtmServiceClient;
        private readonly ITaskStoreLocator _taskStoreLocator;
        private readonly ITaskSynchronizationService _synchronizationService;
        private ITaskStore lastTaskStore;

        private int _activeTaskCollectionIndex;
        private bool _isSyncing;

        private List<Group> _dueByTaskCollection;
        private List<Group> _listTaskGroup;
        private List<Group> _locationTaskGroup;

        private List<Group> _activeTaskCollection;
        private TaskGroupsViewModel _selectedTaskGroups;

        private ObservableCollection<TaskGroupsViewModel> _dueByTaskGroupsViewModels;
        private CollectionViewSource _dueByTaskGroupsViewSource;

        #endregion Members

        #region Constructor(s)

        public TaskCollectionViewModel(
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

            this.ActiveTaskCollectionIndex = 1;
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties
        
        public ICollectionView DueByCollectionViewSource { get { return this._dueByTaskGroupsViewSource.View; } }
        public ICollectionView ListCollectionViewSource { get { return this._dueByTaskGroupsViewSource.View; } }
        public ICollectionView LocationCollectionViewSource { get { return this._dueByTaskGroupsViewSource.View; } }

        public List<Group> ActiveTaskCollection
        {
            get { return this._activeTaskCollection; }
            set
            {
                this._activeTaskCollection = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public int ActiveTaskCollectionIndex
        {
            get { return this._activeTaskCollectionIndex; }

            set
            {
                this._activeTaskCollectionIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskGroupsViewModel SelectedTaskGroups
        {
            get { return this._selectedTaskGroups; }

            set
            {
                if (value != null)
                {
                    this._selectedTaskGroups = value;
                    this.RaisePropertyChanged(() => this.SelectedTaskGroups);
                }
            }
        }

        public ObservableCollection<TaskGroupsViewModel> DueByTaskGroupsViewModels
        {
            get { return this._dueByTaskGroupsViewModels; }

            set
            {
                if (value != null)
                {
                    this._dueByTaskGroupsViewModels = value;
                    this.RaisePropertyChanged(() => this.DueByTaskGroupsViewModels);
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
            //if (this._selectedTaskGroups == null)
            //{
            //    var tombstoned = Tombstoning.Load<TaskGroupViewModel>("SelectedTaskGroups");
            //    if (tombstoned != null)
            //        this.SelectedTaskGroups = new TaskGroupViewModel(
            //            tombstoned.DueByCollection, this.NavigationService, this._taskStoreLocator);
            //}

            //this._activeTaskCollectionIndex = Tombstoning.Load<int>("MainPivot");
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTaskGroups", this.SelectedTaskGroups);
            Tombstoning.Save("MainPivot", this.ActiveTaskCollectionIndex);

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
            this.BuildPanoramaDimensions();
            this.RaisePropertyChanged(string.Empty);
            this.IsSyncing = false;
            this.UpdateCommandsForSync();
        }

        public void Refresh()
        {
            if (this._taskStoreLocator.GetStore() != this.lastTaskStore)
            {
                this.lastTaskStore = this._taskStoreLocator.GetStore();
                this.BuildPanoramaDimensions();
                this.RaisePropertyChanged(string.Empty);
                this.StartSyncCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Methods

        #region Private Methods

        private void BuildPanoramaDimensions()
        {
            var tasks = this._taskStoreLocator.GetStore().GetAllTasks();
            BuildDueByDimensions(tasks);

            // Initialize the selected survey template))
            this.HandleCurrentSectionChanged();
        }

        private void BuildDueByDimensions(List<Model.Task> tasks)
        {
            var startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = DateTime.Now.EndOfWeek(DayOfWeek.Monday);

            var dueTodayTasks = tasks.
                Where(task => ((task.Due.AsDateTime(DateTime.MaxValue) <= DateTime.Today) &&
                                (task.Completed == null) && (task.Deleted == null)));

            var dueTomorrowTasks = tasks.
                Where(task => ((task.Due.AsDateTime(DateTime.MaxValue) == DateTime.Today.AddDays(1)) &&
                                (task.Completed == null) && (task.Deleted == null)));

            var dueThisWeekTasks = tasks.
                Where(task => (task.Due.AsDateTime(DateTime.MaxValue) >= startOfWeek &&
                                task.Due.AsDateTime(DateTime.MaxValue) <= endOfWeek) &&
                               (task.Completed == null) && (task.Deleted == null));

            var dueNextWeekTasks = tasks.
                Where(task => (task.Due.AsDateTime(DateTime.MaxValue) >= startOfWeek.AddDays(7) &&
                                task.Due.AsDateTime(DateTime.MaxValue) <= endOfWeek.AddDays(7)) &&
                               (task.Completed == null) && (task.Deleted == null));

            _dueByTaskCollection = new List<Group> {
                new Group {Name = "Today", Tasks = dueTodayTasks.ToList()},
                new Group {Name = "Tomorrow", Tasks = dueTomorrowTasks.ToList()},
                new Group {Name = "This Week", Tasks = dueThisWeekTasks.ToList()},
                new Group {Name = "Next Week", Tasks = dueNextWeekTasks.ToList()}
            };

            this._dueByTaskGroupsViewModels = new ObservableCollection<TaskGroupsViewModel>();
            var dueByItemViewModels = this._dueByTaskCollection.Select(o => 
                    new TaskGroupViewModel(o.Name, o.Tasks, this.NavigationService, this._taskStoreLocator)).ToList();
            dueByItemViewModels.ForEach(this._dueByTaskGroupsViewModels.Add);

            // Create collection views
            this._dueByTaskGroupsViewSource = new CollectionViewSource { Source = this._dueByTaskGroupsViewModels };

            this._dueByTaskGroupsViewSource.View.CurrentChanged += (o, e) =>
                this.SelectedTaskGroups = (TaskGroupViewModel)this._dueByTaskGroupsViewSource.View.CurrentItem;
        }

        private void HandleCurrentSectionChanged()
        {
            ICollectionView currentView = null;
            switch (this._activeTaskCollectionIndex)
            {
                case 0:
                    currentView = this.DueByCollectionViewSource;
                    this.SelectedTaskGroups = (TaskGroupViewModel)currentView.CurrentItem;
                    break;
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