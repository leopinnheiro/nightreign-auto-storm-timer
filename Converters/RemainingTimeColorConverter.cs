using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace nightreign_auto_storm_timer.Converters
{
    public class RemainingTimeColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush DefaultColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fafafa"));
        private static readonly SolidColorBrush WarningColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f97316"));
        private static readonly SolidColorBrush CriticalColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc2626"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time)
            {
                int seconds = (int)time.TotalSeconds;
                if (seconds == 0)
                    return DefaultColor;

                if (seconds <= 10)
                    return CriticalColor;

                if (seconds <= 30)
                    return WarningColor;
            }

            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
