using SoundBoard.Helpers;

namespace SoundBoard.ViewModel
{
    public class NameDialogViewModel: DialogViewModelBase
    {
        private string _name;
        private bool _isNameValid;
        private string _title;
        private string _prompt;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();

                IsNameValid = !string.IsNullOrEmpty(_name);
            }
        }

        public bool IsNameValid
        {
            get { return _isNameValid; }
            set
            {
                if (value == _isNameValid) return;
                _isNameValid = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Prompt
        {
            get { return _prompt; }
            set
            {
                if (value == _prompt) return;
                _prompt = value;
                OnPropertyChanged();
            }
        }
    }
}