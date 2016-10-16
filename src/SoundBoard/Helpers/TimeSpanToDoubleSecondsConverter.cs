using System;
using System.Globalization;

namespace SoundBoard.Helpers
{
    public class TimeSpanToDoubleSecondsConverter : TypedValueConverterBase<TimeSpan, double?>
    {
        public TimeSpanToDoubleSecondsConverter() : base(Convert, ConvertBack)
        {
            
        }

        private static TimeSpan ConvertBack(double? value, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(value.GetValueOrDefault());
        }

        private static double? Convert(TimeSpan value, object parameter, CultureInfo culture)
        {
            return value.TotalSeconds;
        }
    }
}
