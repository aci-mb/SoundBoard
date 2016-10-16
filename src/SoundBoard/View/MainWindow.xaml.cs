using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

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
    }
}