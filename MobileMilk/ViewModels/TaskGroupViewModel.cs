using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Model;
using MobileMilk.Store;
using System.Collections.Generic;

namespace MobileMilk.ViewModels
{
    public class TaskGroupViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand TaskGroupCommand { get; set; }
        public DelegateCommand TaskCommand { get; set; }

        #endregion Delegates

        #region Members

        private ObservableCollection<TaskViewModel> _observableItems;
        private TaskViewModel _selectedTaskViewModel;
        private int _selectedTaskViewIndex;

        private CollectionViewSource _tasksViewSource;

        #endregion Members

        #region Constructor(s)

        public TaskGroupViewModel(
            string groupName, List<Task> tasks,
            DelegateCommand taskGroupCommand,
            INavigationService navigationService)
            : base(navigationService)
        {
            this.Name = groupName;
            this.Tasks = tasks;
            this.TaskGroupCommand = taskGroupCommand;

            this.TaskCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/TaskDetailsView.xaml", UriKind.Relative)); });

            Load();
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        [DataMember]
        public List<Task> Tasks { get; set; }

        [DataMember]
        public string Name { get; set; }

        public int Count { get { return this.Tasks.Count; } }

        public ICollectionView TasksViewSource { get { return this._tasksViewSource.View; } }

        public ObservableCollection<TaskViewModel> TaskViewModels
        {
            get { return this._observableItems; }

            set
            {
                if (value != null)
                {
                    this._observableItems = value;
                    this.RaisePropertyChanged(() => this.TaskViewModels);
                }
            }
        }

        public int SelectedTaskViewIndex
        {
            get { return this._selectedTaskViewIndex; }

            set
            {
                this._selectedTaskViewIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskViewModel SelectedTaskViewModel
        {
            get { return this._selectedTaskViewModel; }

            set
            {
                if (value != null)
                {
                    this._selectedTaskViewModel = value;
                    this.RaisePropertyChanged(() => this.SelectedTaskViewModel);
                }
            }
        }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            if (this._selectedTaskViewModel == null)
            {
                var tombstoned = Tombstoning.Load<TaskViewModel>("SelectedTaskItem");
                if (tombstoned != null)
                {
                    this.SelectedTaskViewModel = new TaskViewModel(tombstoned.TaskItem, TaskCommand, this.NavigationService);
                }

                this._selectedTaskViewIndex = Tombstoning.Load<int>("SelectedTaskIndex");
            }
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTaskItem", this.SelectedTaskViewModel);
            Tombstoning.Save("SelectedTaskIndex", this.SelectedTaskViewIndex);

            base.IsBeingDeactivated();
        }

        private void Load()
        {
            // Update the View Model status
            this.BuildPivotDimensions();
            this.RaisePropertyChanged(string.Empty);
        }

        #endregion Methods

        #region Private Methods

        private void BuildPivotDimensions()
        {
            this._observableItems = new ObservableCollection<TaskViewModel>();
            var taskListItemViewModels = this.Tasks.Select(t =>
                    new TaskViewModel(t, TaskCommand, this.NavigationService)).ToList();
            taskListItemViewModels.ForEach(this._observableItems.Add);

            // Listen for task changes
            // TODO: listen for task changes
            //this.ListenSurveyChanges();

            // Create collection views
            this._tasksViewSource = new CollectionViewSource { Source = this._observableItems };

            //this._tasksViewSource.Filter += (o, e) => {
            //    var task = (TaskViewModel) e.Item;
            //    e.Accepted = ((task.Due.AsDateTime(DateTime.MaxValue) <= DateTime.Today) &&
            //        (task.Completed == null) && (task.Deleted == null));
            //};

            this._tasksViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));

            this._tasksViewSource.View.CurrentChanged += View_CurrentChanged;
                //(o, e) => this.SelectedTaskViewModel = (TaskViewModel)this._tasksViewSource.View.CurrentItem;

            // Initialize the selected survey template
            this.HandleCurrentSectionChanged();
        }

        void View_CurrentChanged(object sender, EventArgs e)
        {
            this.SelectedTaskViewModel = (TaskViewModel) this._tasksViewSource.View.CurrentItem;
        }

        private void HandleCurrentSectionChanged()
        {
            this.SelectedTaskViewModel = (TaskViewModel)this.TasksViewSource.CurrentItem;
        }

        #endregion Private Methods
    }
}
