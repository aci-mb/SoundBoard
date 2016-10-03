using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AcillatemSoundBoard.Helpers
{
    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return new GridLength((double)value);
            }
            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength)
            {
                return ((GridLength)value).Value;
            }
            throw new InvalidOperationException();
        }
    }
}
