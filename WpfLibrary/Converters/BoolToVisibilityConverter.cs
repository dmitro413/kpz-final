using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace WpfLibrary.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is bool b && b;
            bool invert = parameter?.ToString() == "Invert";
            return (isVisible ^ invert) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Visibility v && v == Visibility.Visible;
    }
}