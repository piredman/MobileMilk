using Microsoft.Phone.Controls;
using MobileMilk.ViewModels;

namespace MobileMilk.Views
{
    public partial class HomeView : PhoneApplicationPage
    {
        public HomeView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var viewModel = this.DataContext as HomeViewModel;
            if (viewModel == null)
                return;

            viewModel.CheckAuthorization();
        }
    }
}