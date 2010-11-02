using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.ViewModels
{
    public class TaskGroupsViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand TaskGroupCommand { get; set; }

        #endregion Delegates

        #region Members

        private ObservableCollection<TaskGroupViewModel> _observableGroups;
        private TaskGroupViewModel _selected;
        private int _selectedIndex;

        private CollectionViewSource _taskGroupsViewSource;

        #endregion Members

        #region Constructor(s)

        public TaskGroupsViewModel(
            string collectionName, List<Group> taskGroups,
            INavigationService navigationService)
            : base(navigationService)
        {
            this.Name = collectionName;
            this.Groups = taskGroups;

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

        public ICollectionView TaskGroupsViewSource { get { return this._taskGroupsViewSource.View; } }

        public int SelectedIndex
        {
            get { return this._selectedIndex; }

            set
            {
                this._selectedIndex = value;
                this.HandleCurrentSectionChanged();
            }
        }

        public TaskGroupViewModel Selected
        {
            get { return this._selected; }

            set
            {
                if (value != null)
                {
                    this._selected = value;
                    this.RaisePropertyChanged(() => this.Selected);
                }
            }
        }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated()
        {
            if (this._selected == null)
            {
                var tombstoned = Tombstoning.Load<TaskGroupViewModel>("SelectedTaskGroup");
                if (tombstoned != null)
                {
                    this.Selected = new TaskGroupViewModel(tombstoned.Name, tombstoned.Tasks, TaskGroupCommand, this.NavigationService);
                }

                this._selectedIndex = Tombstoning.Load<int>("SelectedTaskIndex");
            }
        }

        public override void IsBeingDeactivated()
        {
            Tombstoning.Save("SelectedTaskGroup", this.Selected);
            Tombstoning.Save("SelectedTaskIndex", this.SelectedIndex);

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
            this._observableGroups = new ObservableCollection<TaskGroupViewModel>();
            var groupViewModels = this.Groups.Select(o =>
                    new TaskGroupViewModel(o.Name, o.Tasks, TaskGroupCommand, this.NavigationService)).ToList();
            groupViewModels.ForEach(this._observableGroups.Add);

            // Create collection views
            this._taskGroupsViewSource = new CollectionViewSource { Source = this._observableGroups };

            this._taskGroupsViewSource.View.CurrentChanged += (o, e) =>
                this.Selected = (TaskGroupViewModel)this._taskGroupsViewSource.View.CurrentItem;

            // Initialize the selected survey template
            this.HandleCurrentSectionChanged();
        }

        private void HandleCurrentSectionChanged()
        {
            ICollectionView currentView = null;
            switch (this.SelectedIndex)
            {
                case 0:
                    currentView = this.TaskGroupsViewSource;
                    break;
            }

            if (currentView != null)
            {
                this.Selected = (TaskGroupViewModel)currentView.CurrentItem;
            }
        }

        #endregion Private Methods
    }
}
