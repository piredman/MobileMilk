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
    public class TasksByDueViewModel : ViewModel
    {
        #region Members

        private readonly ITaskStoreLocator _taskStoreLocator;
        private ITaskStore _lastTaskStore;

        private ObservableCollection<TaskListItemViewModel> _observableTaskListItems;
        private TaskListItemViewModel _selectedTaskListItem;
        private int _selectedPivotIndex;

        private CollectionViewSource _todaysTasksViewSource;
        private CollectionViewSource _tomorrowsTasksViewSource;
        private CollectionViewSource _thisWeeksTasksViewSource;
        private CollectionViewSource _nextWeeksTasksViewSource;

        #endregion Members

        #region Constructor(s)

        public TasksByDueViewModel(
            INavigationService navigationService,
            ITaskStoreLocator taskStoreLocator)
            : base(navigationService)
        {
            this._taskStoreLocator = taskStoreLocator;

            Load();
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        public ICollectionView TodaysTasks { get { return this._todaysTasksViewSource.View; } }
        public ICollectionView TomorrowsTasks { get { return this._tomorrowsTasksViewSource.View; } }
        public ICollectionView ThisWeeksTasks { get { return this._thisWeeksTasksViewSource.View; } }
        public ICollectionView NextWeeksTasks { get { return this._nextWeeksTasksViewSource.View; } }

        public int SelectedPivotIndex
        {
            get { return this._selectedPivotIndex; }

            set
            {
                this._selectedPivotIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskListItemViewModel SelectedTaskListItem
        {
            get { return this._selectedTaskListItem; }

            set
            {
                if (value != null)
                {
                    this._selectedTaskListItem = value;
                    this.RaisePropertyChanged(() => this.SelectedTaskListItem);
                }
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
            if (this._selectedTaskListItem == null)
            {
                var tombstoned = Tombstoning.Load<TaskListItemViewModel>("SelectedTemplate");
                if (tombstoned != null)
                {
                    this.SelectedTaskListItem = new TaskListItemViewModel(tombstoned.TaskItem, this.NavigationService);
                }

                this._selectedPivotIndex = Tombstoning.Load<int>("MainPivot");
            }
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTemplate", this.SelectedTaskListItem);
            Tombstoning.Save("MainPivot", this.SelectedPivotIndex);

            base.IsBeingDeactivated();
        }

        private void Load()
        {
            // Update the View Model status
            this.BuildPivotDimensions();
            this.RaisePropertyChanged(string.Empty);
        }

        public void Refresh()
        {
            if (this._taskStoreLocator.GetStore() != this._lastTaskStore)
            {
                this._lastTaskStore = this._taskStoreLocator.GetStore();
                this.BuildPivotDimensions();
                this.RaisePropertyChanged(string.Empty);
            }
        }

        #endregion Methods

        #region Private Methods

        private void BuildPivotDimensions()
        {
            this._observableTaskListItems = new ObservableCollection<TaskListItemViewModel>();
            var taskListItemViewModels = this._taskStoreLocator.GetStore().GetAllTasks().Select(t => 
                    new TaskListItemViewModel(t, this.NavigationService)).ToList();
            taskListItemViewModels.ForEach(this._observableTaskListItems.Add);

            // Listen for task changes
            // TODO: listen for task changes
            //this.ListenSurveyChanges();

            // Create collection views
            this._todaysTasksViewSource = new CollectionViewSource { Source = this._observableTaskListItems };
            this._tomorrowsTasksViewSource = new CollectionViewSource { Source = this._observableTaskListItems };
            this._thisWeeksTasksViewSource = new CollectionViewSource { Source = this._observableTaskListItems };
            this._nextWeeksTasksViewSource = new CollectionViewSource { Source = this._observableTaskListItems };

            var startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = DateTime.Now.EndOfWeek(DayOfWeek.Monday);

            this._todaysTasksViewSource.Filter += (o, e) => {
                var task = (TaskListItemViewModel) e.Item;
                e.Accepted = ((task.Due.AsDateTime(DateTime.MaxValue) <= DateTime.Today) &&
                    (task.Completed == null) && (task.Deleted == null));
            };
            this._tomorrowsTasksViewSource.Filter += (o, e) => {
                var task = (TaskListItemViewModel)e.Item;
                e.Accepted = ((task.Due.AsDateTime(DateTime.MaxValue) == DateTime.Today.AddDays(1)) &&
                    (task.Completed == null) && (task.Deleted == null));
            };
            this._thisWeeksTasksViewSource.Filter += (o, e) => {
                var task = (TaskListItemViewModel)e.Item;
                e.Accepted = (
                    (task.Due.AsDateTime(DateTime.MaxValue) >= startOfWeek &&
                     task.Due.AsDateTime(DateTime.MaxValue) <= endOfWeek) && 
                    (task.Completed == null) && (task.Deleted == null)
                );
            };
            this._nextWeeksTasksViewSource.Filter += (o, e) => {
                var task = (TaskListItemViewModel)e.Item;
                e.Accepted = (
                    (task.Due.AsDateTime(DateTime.MaxValue) >= startOfWeek.AddDays(7) && 
                     task.Due.AsDateTime(DateTime.MaxValue) <= endOfWeek.AddDays(7)) &&
                    (task.Completed == null) && (task.Deleted == null)
                );
            };

            this._todaysTasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));
            this._tomorrowsTasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));
            this._thisWeeksTasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));
            this._nextWeeksTasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));

            this._todaysTasksViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedTaskListItem = (TaskListItemViewModel)this._todaysTasksViewSource.View.CurrentItem;
            this._tomorrowsTasksViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedTaskListItem = (TaskListItemViewModel)this._tomorrowsTasksViewSource.View.CurrentItem;
            this._thisWeeksTasksViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedTaskListItem = (TaskListItemViewModel)this._thisWeeksTasksViewSource.View.CurrentItem;
            this._nextWeeksTasksViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedTaskListItem = (TaskListItemViewModel)this._nextWeeksTasksViewSource.View.CurrentItem;

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

        #endregion Private Methods
    }
}
