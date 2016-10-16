using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SoundBoard.Helpers
{
    public class ErrorInfo : INotifyPropertyChanged
    {
        private bool _hasError;

        public bool HasError
        {
            get { return _hasError; }
            set
            {
                if (value.Equals(_hasError)) return;
                _hasError = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
	        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
