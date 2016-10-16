using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoundBoard.Helpers
{
    public class TypedValueConverterBase<TSourceType, TTargetType> : IValueConverter
    {
        private readonly ConvertFunc _convertFunc;
        private readonly ConvertBackFunc _convertBackFunc;

        public delegate TTargetType ConvertFunc(TSourceType value, object parameter, CultureInfo culture);

        public delegate TSourceType ConvertBackFunc(TTargetType value, object parameter, CultureInfo culture);

        public TypedValueConverterBase(ConvertFunc convertFunc, ConvertBackFunc convertBackFunc = null)
        {
            _convertFunc = convertFunc;
            _convertBackFunc = convertBackFunc;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TSourceType))
            {
                throw new ArgumentException(
                    string.Format("Value must be of type {0}", typeof (TSourceType)),
                    "value");
            }
            if (targetType != typeof (TTargetType))
            {
                throw new ArgumentException(
                    string.Format("TargetType must be {0}", typeof (TTargetType)),
                    "targetType");
            }
            return _convertFunc((TSourceType) value, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TTargetType))
            {
                throw new ArgumentException(
                    string.Format("Value must be of type {0}", typeof (TTargetType)),
                    "value");
            }
            if (targetType != typeof (TSourceType))
            {
                throw new ArgumentException(
                    string.Format("TargetType must be {0}", typeof (TSourceType)),
                    "targetType");
            }
            return _convertBackFunc != null
                ? _convertBackFunc((TTargetType) value, parameter, culture)
                : DependencyProperty.UnsetValue;
        }
    }
}
