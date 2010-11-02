using Microsoft.Phone.Controls;
using MobileMilk.ViewModels;

namespace MobileMilk.Views
{
    public partial class TaskCollectionsView : PhoneApplicationPage
    {
        public TaskCollectionsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var viewModel = this.DataContext as TaskCollectionsViewModel;
            if (viewModel == null)
                return;

            viewModel.CheckAuthorization();
        }
    }
}