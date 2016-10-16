using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundBoard.ViewModel.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        private const double FalseOpacity = 0.4;
        private const double TrueOpacity = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue;
            try
            {
                booleanValue = System.Convert.ToBoolean(value);
            }
            catch
            {
                booleanValue = false;
            }

            return booleanValue ? TrueOpacity : FalseOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacity;
            try
            {
                opacity = System.Convert.ToDouble(value);
            }
            catch
            {
                opacity = FalseOpacity;
            }

            return opacity == TrueOpacity;
        }
    }
}