using SoundBoard.Helpers;

namespace SoundBoard.ViewModel
{
    public class QuestionDialogViewModel : DialogViewModelBase
    {
        private string _title;
        private string _question;

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

        public string Question
        {
            get { return _question; }
            set
            {
                if (value == _question) return;
                _question = value;
                OnPropertyChanged();
            }
        }
    }
}