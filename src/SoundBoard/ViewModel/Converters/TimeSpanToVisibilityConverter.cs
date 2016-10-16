using System;
using System.Globalization;
using System.Windows;
using SoundBoard.Helpers;

namespace SoundBoard.ViewModel.Converters
{
    public class TimeSpanToVisibilityConverter : TypedValueConverterBase<TimeSpan, Visibility>
    {
        public TimeSpanToVisibilityConverter() : base(Convert, null)
        {
            
        }

        private static Visibility Convert(TimeSpan value, object parameter, CultureInfo culture)
        {
            return value <= TimeSpan.Zero
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}