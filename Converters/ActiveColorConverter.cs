using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace nightreign_auto_storm_timer.Converters
{
    public class ActiveColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush DefaultColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fafafa"));
        private static readonly SolidColorBrush ActiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eab308"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isActive = (bool)value;

            if (isActive)
            {
                return ActiveColor;
            }

            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
