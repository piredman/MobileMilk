using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MobileMilk.Resources.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public bool Negative { get; set; }

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var visibility = (bool)value;
            return ((this.Negative && !visibility) || (!this.Negative && visibility)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return this.Negative ? visibility != Visibility.Visible : visibility == Visibility.Visible;
        }
    }
}