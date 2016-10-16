using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoundBoard.Helpers
{
    public class BooleanToToggleTextConverter : IValueConverter
    {
        private readonly string _trueText;
        private readonly string _falseText;

        public BooleanToToggleTextConverter(string trueText, string falseText)
        {
            _trueText = trueText;
            _falseText = falseText;
        }

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

            return booleanValue ?_trueText : _falseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}