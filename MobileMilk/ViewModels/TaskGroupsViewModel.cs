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
using MobileMilk.ViewModels.Task;

namespace MobileMilk.ViewModels
{
    public class TaskGroupsViewModel : ViewModel
    {
        #region Delegates
        #endregion Delegates

        #region Members
        
        private ObservableCollection<TaskGroupViewModel> _observableGroups;
        private TaskViewModel _selectedGroup;
        private int _selectedGroupIndex;

        private CollectionViewSource _groupsViewSource;

        #endregion Members

        #region Constructor(s)

        public TaskGroupsViewModel(
            string name, List<Group> taskCollection,
            INavigationService navigationService)
            : base(navigationService)
        {
            this.Name = name;
            this.Groups = taskCollection;

            Load();
            this.IsBeingActivated();
        }

        #endregion Constructor(s)

        #region Properties

        [DataMember]
        public List<Group> Groups { get; set; }

        [DataMember]
        public string Name { get; set; }

        public int Count { get { return this.Groups.Count; } }

        public ICollectionView TaskGroupsViewSource { get { return this._groupsViewSource.View; } }

        public int SelectedGroupIndex
        {
            get { return this._selectedGroupIndex; }

            set
            {
                this._selectedGroupIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskViewModel SelectedGroup
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

        #endregion Properties

        #region Methods

        public override void IsBeingActivated() { }

        public override void IsBeingDeactivated()
        {
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
            this._observableGroups = new ObservableCollection<TaskViewModel>();
            var taskListItemViewModels = this.Groups.Select(t => 
                    new TaskViewModel(t, this.NavigationService)).ToList();
            taskListItemViewModels.ForEach(this._observableGroups.Add);

            // Listen for task changes
            // TODO: listen for task changes
            //this.ListenSurveyChanges();

            // Create collection views
            this._groupsViewSource = new CollectionViewSource { Source = this._observableGroups };

            //this._tasksViewSource.Filter += (o, e) => {
            //    var task = (TaskViewModel) e.Item;
            //    e.Accepted = ((task.Due.AsDateTime(DateTime.MaxValue) <= DateTime.Today) &&
            //        (task.Completed == null) && (task.Deleted == null));
            //};

            this._groupsViewSource.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));

            this._groupsViewSource.View.CurrentChanged +=
                (o, e) => this.SelectedGroup = (TaskViewModel)this._groupsViewSource.View.CurrentItem;

            // Initialize the selected survey template
            this.HandleCurrentSectionChanged();
        }

        private void HandleCurrentSectionChanged()
        {
            ICollectionView currentView = null;
            switch (this.SelectedGroupIndex)
            {
                case 0:
                    currentView = this.TaskGroupsViewSource;
                    break;
            }

            if (currentView != null)
            {
                this.SelectedGroup = (TaskViewModel)currentView.CurrentItem;
            }
        }

        #endregion Private Methods
    }
}
