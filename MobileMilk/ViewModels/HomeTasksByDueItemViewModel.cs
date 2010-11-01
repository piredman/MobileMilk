using System;
using System.Runtime.Serialization;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;

namespace MobileMilk.ViewModels
{
    [DataContract]
    public class HomeTasksByDueItemViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand TasksByDueCommand { get; set; }

        #endregion Delegates

        #region Members
        #endregion Members

        public HomeTasksByDueItemViewModel(string name, int count, INavigationService navigationService) 
            : base(navigationService)
        {
            Name = name;
            Count = count;
            
            this.TasksByDueCommand = new DelegateCommand(
                () => { this.NavigationService.Navigate(new Uri("/Views/TasksByDueView.xaml", UriKind.Relative)); });

            this.IsBeingActivated();
        }

        #region Properties

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Count { get; set; }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated() {}

        #endregion Methods
    }
}
