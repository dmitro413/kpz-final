using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfLibrary.Converters
{
    public class NumberToColorConverter : IValueConverter
    {
        private static readonly Brush[] NumberColors =
        {
            Brushes.Transparent,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Red,
            Brushes.DarkBlue,
            Brushes.DarkRed,
            Brushes.Teal,
            Brushes.Black,
            Brushes.Gray,
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number && number >= 0 && number < NumberColors.Length)
                return NumberColors[number];

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}