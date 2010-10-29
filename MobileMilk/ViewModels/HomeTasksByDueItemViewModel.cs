using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using MobileMilk.Common;
using MobileMilk.Model;

namespace MobileMilk.ViewModels
{
    [DataContract]
    public class HomeTasksByDueItemViewModel : ViewModel
    {
        #region Members
        #endregion Members

        public HomeTasksByDueItemViewModel(string name, int count, INavigationService navigationService) 
            : base(navigationService)
        {
            Name = name;
            Count = count;

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
