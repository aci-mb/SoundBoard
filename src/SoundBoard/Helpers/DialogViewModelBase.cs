using System.Windows.Input;

namespace SoundBoard.Helpers
{
    public class DialogViewModelBase : ViewModelBase
    {
        private ICommand _okCommand;
        private ICommand _cancelCommand;
        private bool? _dialogResult;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set
            {
                if (value.Equals(_dialogResult)) return;
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new DelegateCommand(o =>  DialogResult = true);
                }
                return _okCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new DelegateCommand(o => DialogResult = false);
                }
                return _cancelCommand;
            }
        }
    }
}