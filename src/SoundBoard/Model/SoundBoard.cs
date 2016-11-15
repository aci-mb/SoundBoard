using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SoundBoard.Annotations;

namespace SoundBoard.Model
{
    public class SoundBoard : INotifyPropertyChanged
    {
        private ObservableCollection<ISound> _sounds;

        private string _name;

        public SoundBoard()
        {
            Sounds = new ObservableCollection<ISound>();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ISound> Sounds
        {
            get { return _sounds; }
            set
            {
                if (Equals(value, _sounds)) return;
                _sounds = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
	        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}