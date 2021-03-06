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
    public class TaskCollectionsViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand ViewTaskCollectionCommand { get; set; }
        public DelegateCommand StartSyncCommand { get; set; }
        public DelegateCommand AppSettingsCommand { get; set; }

        public DelegateCommand AddTaskCommand { get; set; }
        public DelegateCommand RefreshTasksCommand { get; set; }
                
        #endregion Delegates
        
        #region Members

        private readonly InteractionRequest<Notification> submitErrorInteractionRequest;
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        private readonly IRtmServiceClient _rtmServiceClient;
        private readonly ITaskStoreLocator _taskStoreLocator;
        private readonly IListStoreLocator _listStoreLocator;
        private readonly ILocationStoreLocator _locationStoreLocator;
        private readonly ISynchronizationService _synchronizationService;
        private ITaskStore lastTaskStore;

        // Panorama Sources
        private List<Group> _dueByCollection;
        private List<Group> _listCollection;
        private List<Group> _locationCollection;

        private ObservableCollection<TaskGroupViewModel> _dueByCollectionViewModels;
        private CollectionViewSource _dueByCollectionViewSource;
        private ObservableCollection<TaskGroupViewModel> _listCollectionViewModels;
        private CollectionViewSource _listCollectionViewSource;
        private ObservableCollection<TaskGroupViewModel> _locationCollectionViewModels;
        private CollectionViewSource _locationCollectionViewSource;

        private bool _isSyncing;
        private bool _isEditing;

        private ICollectionView _selectedCollectionViewSource;
        private int _selectedCollectionIndex;

        private int _dueBySelectedIndex;
        private int _listSelectedIndex;
        private int _locationSelectedIndex;

        private TaskGroupViewModel _selectedGroup;
        private int _selectedGroupIndex;

        #endregion Members

        #region Constructor(s)

        public TaskCollectionsViewModel(
            INavigationService navigationService,
            IRtmServiceClient rtmServiceClient,
            ITaskStoreLocator taskStoreLocator,
            IListStoreLocator listStoreLocator,
            ILocationStoreLocator locationStoreLocator,
            ISynchronizationService synchronizationService)
            : base(navigationService)
        {
            this._rtmServiceClient = rtmServiceClient;
            this._taskStoreLocator = taskStoreLocator;
            this._listStoreLocator = listStoreLocator;
            this._locationStoreLocator = locationStoreLocator;
            this._synchronizationService = synchronizationService;

            this.submitErrorInteractionRequest = new InteractionRequest<Notification>();
            this.submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            this.StartSyncCommand = new DelegateCommand(
                () => { this.StartSync(); },
                () => !this.IsSyncing && !this.SettingAreNotConfigured);

            this.ViewTaskCollectionCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/TaskCollectionView.xaml", UriKind.Relative)); },
                () => !this.IsSyncing);

            this.AppSettingsCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/AppSettingsView.xaml", UriKind.Relative)); },
                () => !this.IsSyncing);

            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public ICollectionView DueByCollectionViewSource { get { return this._dueByCollectionViewSource.View; } }
        public ICollectionView ListCollectionViewSource { get { return this._listCollectionViewSource.View; } }
        public ICollectionView LocationCollectionViewSource { get { return this._locationCollectionViewSource.View; } }

        public ObservableCollection<TaskGroupViewModel> DueByCollectionViewModels
        {
            get { return this._dueByCollectionViewModels; }
            set
            {
                if (value != null)
                {
                    this._dueByCollectionViewModels = value;
                    this.RaisePropertyChanged(() => this.DueByCollectionViewModels);
                }
            }
        }
        public ObservableCollection<TaskGroupViewModel> ListCollectionViewModels
        {
            get { return this._listCollectionViewModels; }

            set
            {
                if (value != null)
                {
                    this._listCollectionViewModels = value;
                    this.RaisePropertyChanged(() => this.ListCollectionViewModels);
                }
            }
        }
        public ObservableCollection<TaskGroupViewModel> LocationCollectionViewModels
        {
            get { return this._locationCollectionViewModels; }

            set
            {
                if (value != null)
                {
                    this._locationCollectionViewModels = value;
                    this.RaisePropertyChanged(() => this.LocationCollectionViewModels);
                }
            }
        }
        
        public ICollectionView SelectedCollectionViewSource
        {
            get { return this._selectedCollectionViewSource; }
            set
            {
                this._selectedCollectionViewSource = value;
                this.RaisePropertyChanged(() => this.SelectedCollectionViewSource);
            }
        }
        public string SelectedCollectionName { get; set; }
        public int SelectedCollectionIndex
        {
            get { return this._selectedCollectionIndex; }
            set
            {
                if (this._selectedCollectionIndex == value)
                    return;

                this._selectedCollectionIndex = value;
                this.HandleCollectionSectionChanged();
            }
        }

        public TaskGroupViewModel SelectedGroup
        {
            get { return this._selectedGroup; }
            set
            {
                if (value != null)
                {
                    this._selectedGroup = value;
                    this.RaisePropertyChanged(() => this.SelectedGroup);
                }
            }
        }
        public int SelectedGroupIndex
        {
            get { return this._selectedGroupIndex; }
            set
            {
                if (this._selectedGroupIndex == value)
                    return;

                this._selectedGroupIndex = value;
                HandleGroupSelectionChanged();
            }
        }

        public int DueBySelectedIndex
        {
            get { return this._dueBySelectedIndex; }
            set
            {
                this._dueBySelectedIndex = value;
                this.RaisePropertyChanged(() => this.DueBySelectedIndex);
            }
        }
        public int ListSelectedIndex
        {
            get { return this._listSelectedIndex; }
            set
            {
                this._listSelectedIndex = value;
                this.RaisePropertyChanged(() => this.ListSelectedIndex);
            }
        }
        public int LocationSelectedIndex
        {
            get { return this._locationSelectedIndex; }
            set
            {
                this._locationSelectedIndex = value;
                this.RaisePropertyChanged(() => this.LocationSelectedIndex);
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
            this.SelectedCollectionIndex = Tombstoning.Load<int>("SelectedCollectionIndex");
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedCollectionIndex", this.SelectedCollectionIndex);

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
            BuildListDimensions(tasks);
            BuildLocationDimensions(tasks);

            HandleCollectionSectionChanged();
        }

        private void BuildDueByDimensions(List<Task> tasks)
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

            _dueByCollection = new List<Group> {
                new Group {Name = "Today", Order = 0, Tasks = dueTodayTasks.ToList()},
                new Group {Name = "Tomorrow", Order = 1, Tasks = dueTomorrowTasks.ToList()},
                new Group {Name = "This Week", Order = 2, Tasks = dueThisWeekTasks.ToList()},
                new Group {Name = "Next Week", Order = 3, Tasks = dueNextWeekTasks.ToList()}
            };

            this._dueByCollectionViewModels = new ObservableCollection<TaskGroupViewModel>();
            var viewModels = this._dueByCollection.Select(o =>
                    new TaskGroupViewModel(o.Name, o.Order, o.Tasks, ViewTaskCollectionCommand,
                        this.NavigationService, this._synchronizationService)).ToList();
            viewModels.ForEach(this._dueByCollectionViewModels.Add);

            // Create collection views
            this._dueByCollectionViewSource = new CollectionViewSource { Source = this._dueByCollectionViewModels };
            this._dueByCollectionViewSource.View.CurrentChanged += (o, args) => {
                this.SelectedGroup = (TaskGroupViewModel)this._dueByCollectionViewSource.View.CurrentItem;
                this.SelectedGroupIndex = this._dueByCollectionViewSource.View.CurrentPosition;
            };
        }

        private void BuildListDimensions(List<Task> tasks)
        {
            var lists = this._listStoreLocator.GetStore().GetAllLists();

            //TODO: add smart lists back once filtering for tasks is working
            _listCollection = lists
                .Where(list => list.Smart == false)
                .Select(list => new Group {
                    Name = list.Name, Order = list.Smart ? 1 : 0, Tasks = tasks.Where( task => 
                        ((task.ListId == list.Id) && (task.Completed == null) && (task.Deleted == null))
                    ).ToList()
                }).ToList();

            this._listCollectionViewModels = new ObservableCollection<TaskGroupViewModel>();
            var viewModels = this._listCollection.Select(o =>
                    new TaskGroupViewModel(o.Name, o.Order, o.Tasks, ViewTaskCollectionCommand,
                        this.NavigationService, this._synchronizationService)).ToList();
            viewModels.ForEach(this._listCollectionViewModels.Add);

            // Create collection views
            this._listCollectionViewSource = new CollectionViewSource { Source = this._listCollectionViewModels };

            this._listCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            this._listCollectionViewSource.View.CurrentChanged += (o, args) => {
                this.SelectedGroup = (TaskGroupViewModel)this._listCollectionViewSource.View.CurrentItem;
                this.SelectedGroupIndex = this._listCollectionViewSource.View.CurrentPosition;
            };
        }

        private void BuildLocationDimensions(List<Task> tasks)
        {
            var locations = this._locationStoreLocator.GetStore().GetAllLocations();

            _locationCollection = locations.Select(location => new Group {
                Name = location.Name, Order = 0, Tasks = tasks.Where(task => 
                    ((task.LocationId == location.Id) && (task.Completed == null) && (task.Deleted == null))
                ).ToList()
            }).ToList();

            this._locationCollectionViewModels = new ObservableCollection<TaskGroupViewModel>();
            var viewModels = this._locationCollection.Select(o =>
                    new TaskGroupViewModel(o.Name, o.Order, o.Tasks, ViewTaskCollectionCommand,
                        this.NavigationService, this._synchronizationService)).ToList();
            viewModels.ForEach(this._locationCollectionViewModels.Add);

            // Create collection views
            this._locationCollectionViewSource = new CollectionViewSource { Source = this._locationCollectionViewModels };
            this._locationCollectionViewSource.View.CurrentChanged += (o, args) => {
                this.SelectedGroup = (TaskGroupViewModel)this._locationCollectionViewSource.View.CurrentItem;
                this.SelectedGroupIndex = this._locationCollectionViewSource.View.CurrentPosition;
            };
        }
        
        private void HandleCollectionSectionChanged()
        {
            var selectedIndex = -1;
            _selectedCollectionViewSource = null;
            switch (this.SelectedCollectionIndex)
            {
                case 0:
                    SelectedCollectionName = "Due By";
                    _selectedCollectionViewSource = this.DueByCollectionViewSource;
                    selectedIndex = this._locationCollectionViewSource.View.CurrentPosition;
                    break;
                case 1:
                    SelectedCollectionName = "Lists";
                    _selectedCollectionViewSource = this.ListCollectionViewSource;
                    selectedIndex = this.ListCollectionViewSource.CurrentPosition;
                    break;
                case 2:
                    SelectedCollectionName = "Locations";
                    _selectedCollectionViewSource = this.LocationCollectionViewSource;
                    selectedIndex = this.LocationCollectionViewSource.CurrentPosition;
                    break;
            }

            if (_selectedCollectionViewSource != null)
            {
                this.SelectedGroup = (TaskGroupViewModel)_selectedCollectionViewSource.CurrentItem;
                this.SelectedGroupIndex = selectedIndex;
            }
        }

        private void HandleGroupSelectionChanged()
        {
            if (_selectedCollectionViewSource != null)
            {
                this._selectedCollectionViewSource.MoveCurrentToPosition(this.SelectedGroupIndex);
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
                        case SynchronizationService.GetTasksTask:
                            return string.Format(
                                CultureInfo.InvariantCulture,
                                "syncronized {0} tasks from RTM",
                                summary.Context);
                        case SynchronizationService.SaveTasksTask:
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
