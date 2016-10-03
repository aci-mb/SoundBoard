using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace AcillatemSoundBoard.Helpers
{
    public class ListViewStretchedColumnWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ListView listView = values[1] as ListView;
            GridView gridView = listView == null ? null : listView.View as GridView;
            GridViewColumn stretchedColumn = values[2] as GridViewColumn;

            if (listView == null || gridView == null || stretchedColumn == null)
            {
                throw new NotSupportedException();
            }

            double usedWidth = gridView.Columns
                .Where(column => !ReferenceEquals(column, stretchedColumn))
                .Sum(column => column.ActualWidth);

            return listView.ActualWidth - usedWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] {};
        }
    }
}
