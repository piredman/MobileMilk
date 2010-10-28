using System;
using MobileMilk.Common;

namespace MobileMilk.ViewModels
{
    public class TaskDetailViewModel : ViewModel
    {
        public TaskDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public override void IsBeingActivated()
        {
            throw new NotImplementedException();
        }
    }
}
