using System;
using System.Windows;

namespace SoundBoard.Helpers
{
    public class OpenWindowCommand<T> : DelegateCommand where T : Window, new()
    {
        private readonly object _viewModel;

        public OpenWindowCommand(object viewModel, Predicate<object> canExecute)
            : base(o => { }, canExecute)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            Window window = new T();
            window.DataContext = _viewModel;
            window.ShowDialog();
        }
    }
}