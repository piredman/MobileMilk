using Microsoft.Phone.Controls;
using MobileMilk.ViewModels;

namespace MobileMilk.Views
{
    public partial class TaskCollectionView : PhoneApplicationPage
    {
        public TaskCollectionView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var viewModel = this.DataContext as TaskCollectionViewModel;
            if (viewModel == null)
                return;

            viewModel.CheckAuthorization();
        }
    }
}