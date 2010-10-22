using System;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Data;

namespace MobileMilk.ViewModels
{
    public class AuthorizeViewModel : ViewModel
    {
        public DelegateCommand AuthorizeRtmCommand { get; set; }

        public AuthorizeViewModel(INavigationService navigationService) : base(navigationService)
        {
            this.AuthorizeRtmCommand = new DelegateCommand(NavigateToRtmAuthenticationPage);

            this.IsBeingActivated();
        }

        public override void IsBeingActivated()
        {
            throw new NotImplementedException();
        }

        public void NavigateToRtmAuthenticationPage()
        {
            var authorizationService = new Authorization();
            var frob = authorizationService.GetFrob();
            
            authorizationBrowser
        }
    }
}
