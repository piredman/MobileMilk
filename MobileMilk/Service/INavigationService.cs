using System;

namespace MobileMilk.Common
{
    public interface INavigationService
    {
        bool CanGoBack { get; }
        Uri CurrentSource { get; }
        bool Navigate(Uri source);
        void GoBack();
    }
}
