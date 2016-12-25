using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static System.Double;

namespace SoundBoard.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
			InitializeComponent();
		}

		private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			GridView gridView = ((sender as ListView)?.View as GridView);

			if (gridView == null)
			{
				return;
			}

			// If the column is automatically sized, change the column width to re-apply automatic width
			foreach (GridViewColumn column in gridView.Columns
				.Skip(1)
				.Where(column => IsNaN(column.Width)))
			{
				column.Width = column.ActualWidth;
				column.Width = NaN;
			}
		}
	}
}