using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfLibrary.Converters
{
    public class CellBackgroundConverter : IMultiValueConverter
    {
        private static readonly Brush HiddenBrush = new SolidColorBrush(Color.FromRgb(189, 189, 189));
        private static readonly Brush RevealedBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        private static readonly Brush MineBrush = new SolidColorBrush(Color.FromRgb(244, 67, 54));
        private static readonly Brush FlaggedBrush = new SolidColorBrush(Color.FromRgb(255, 193, 7));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return Brushes.Gray;

            bool isRevealed = values[0] is bool r && r;
            bool isMine = values[1] is bool m && m;
            bool isFlagged = values[2] is bool f && f;

            if (isFlagged) return FlaggedBrush;
            if (!isRevealed) return HiddenBrush;
            if (isMine) return MineBrush;
            return RevealedBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
