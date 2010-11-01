using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Store;
using System.Collections.Generic;
using MobileMilk.ViewModels.Task;

namespace MobileMilk.ViewModels
{
    public class TaskGroupViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand TasksByDueCommand { get; set; }

        #endregion Delegates

        #region Members

        private readonly ITaskStoreLocator _taskStoreLocator;
        private ITaskStore _lastTaskStore;

        private ObservableCollection<TaskItemViewModel> _observableItems;
        private TaskItemViewModel _selectedItem;
        private int _selectedIndex;

        private CollectionViewSource _taskCollectionViewSource;

        #endregion Members

        #region Constructor(s)

        public TaskGroupViewModel(
            string name, List<Model.Task> taskCollection,
            INavigationService navigationService,
            ITaskStoreLocator taskStoreLocator)
            : base(navigationService)
        {
            this.Name = name;
            this.TaskCollection = taskCollection;
            this._taskStoreLocator = taskStoreLocator;

            this.TasksByDueCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/TasksByDueView.xaml", UriKind.Relative)); });

            Load();
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        [DataMember]
        public List<Model.Task> TaskCollection { get; set; }

        [DataMember]
        public string Name { get; set; }

        public int Count { get { return this.TaskCollection.Count; } }

        public ICollectionView TaskCollectionViewSource { get { return this._taskCollectionViewSource.View; } }

        public int SelectedIndex
        {
            get { return this._selectedIndex; }

            set
            {
                this._selectedIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskItemViewModel SelectedItem
        {
            get { return this._selectedItem; }

            set
            {
                if (value != null)
                {
                    this._selectedItem = value;
                    this.RaisePropertyChanged(() => this.SelectedItem);
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
            if (this._selectedItem == null)
            {
                var tombstoned = Tombstoning.Load<TaskItemViewModel>("SelectedTaskItem");
                if (tombstoned != null)
                {
                    this.SelectedItem = new TaskItemViewModel(tombstoned.TaskItem, this.NavigationService);
                }

                this._selectedIndex = Tombstoning.Load<int>("SelectedTaskIndex");
            }
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTaskItem", this.SelectedItem);
            Tombstoning.Save("SelectedTaskIndex", this.SelectedIndex);

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
            this._observableItems = new ObservableCollection<TaskItemViewModel>();
            var taskListItemViewModels = this.TaskCollection.Select(t => 
                    new TaskItemViewModel(t, this.NavigationService)).ToList();
            taskListItemViewModels.ForEach(this._observableItems.Add);

            // Listen for task changes
            // TODO: listen for task changes
            //this.ListenSurveyChanges();

            // Create collection views
            this._taskCollectionViewSource = new CollectionViewSource { Source = this._observableItems };

            //this._tasksViewSource.Filter += (o, e) => {
            //    var task = (TaskItemViewModel) e.Item;
            //    e.Accepted = ((task.Due.AsDateTime(DateTime.MaxValue) <= DateTime.Today) &&
            //        (task.Completed == null) && (task.Deleted == null));
            //};

            this._taskCollectionViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));

            this._taskCollectionViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedItem = (TaskItemViewModel)this._taskCollectionViewSource.View.CurrentItem;

            // Initialize the selected survey template
            this.HandleCurrentSectionChanged();
        }

        private void HandleCurrentSectionChanged()
        {
            ICollectionView currentView = null;
            switch (this.SelectedIndex)
            {
                case 0:
                    currentView = this.TaskCollectionViewSource;
                    break;
            }

            if (currentView != null)
            {
                this.SelectedItem = (TaskItemViewModel)currentView.CurrentItem;
            }
        }

        #endregion Private Methods
    }
}
