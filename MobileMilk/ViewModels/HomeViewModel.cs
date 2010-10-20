using System;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;

namespace MobileMilk.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public DelegateCommand AppSettingsCommand { get; set; }

        private bool isSyncing;

        public HomeViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            this.AppSettingsCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/AppSettingsView.xaml", UriKind.Relative)); },
                () => !this.IsSynchronizing);

            this.IsBeingActivated();
        }

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

        public bool IsSynchronizing
        {
            get { return this.isSyncing; }
            set
            {
                this.isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSynchronizing);
            }
        }
    }
}
