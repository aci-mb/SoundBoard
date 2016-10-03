using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AcillatemSoundBoard.Annotations;
using AcillatemSoundBoard.Helpers;
using AcillatemSoundBoard.Properties;

namespace AcillatemSoundBoard.Model
{
    public class Sound : ISound
    {
        private string _fileName;
        private int _volumeInPercent;
        private string _name;
        private bool _isLooped;
        private TimeSpan _delay;
        private ErrorInfo _errorInfo;

        public Sound()
        {
            _volumeInPercent = 100;
            ErrorInfo = new ErrorInfo();
        }

        public virtual string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == _fileName) return;
                _fileName = value;

                if (string.IsNullOrEmpty(Name))
                {
                    Name = Path.GetFileNameWithoutExtension(FileName);
                }
                OnPropertyChanged();
                VerifyState();
            }
        }

        public ErrorInfo ErrorInfo
        {
            get { return _errorInfo; }
            set
            {
                if (Equals(value, _errorInfo)) return;
                _errorInfo = value;
                OnPropertyChanged();
            }
        }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public virtual int VolumeInPercent
        {
            get { return _volumeInPercent; }
            set
            {
                if (value == _volumeInPercent) return;
                _volumeInPercent = value;
                OnPropertyChanged();
            }
        }

        public virtual bool IsLooped
        {
            get { return _isLooped; }
            set
            {
                if (value.Equals(_isLooped)) return;
                _isLooped = value;
                OnPropertyChanged();
            }
        }

	    public bool IsPlaying { get; }

	    public void Play()
	    {
	    }

	    public void Pause()
	    {
	    }

	    public virtual TimeSpan Delay
        {
            get { return _delay; }
            set
            {
                if (value.Equals(_delay)) return;
                _delay = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return new Sound
            {
                FileName = FileName,
                IsLooped = IsLooped,
                Name = Name,
                VolumeInPercent = VolumeInPercent,
                Delay = Delay
            };
        }

        private void VerifyState()
        {
            if (!DetermineIfFileExists())
            {
                ErrorInfo.ErrorMessage = string.Format(Resources.Sound_VerifyState_The_file___0___does_not_exist, FileName);
                ErrorInfo.HasError = true;
            }
            else
            {
                ErrorInfo.ErrorMessage = string.Empty;
                ErrorInfo.HasError = false;
            }
        }

        private bool DetermineIfFileExists()
        {
            try
            {
                return File.Exists(FileName);
            }
            catch
            {
                return false;
            }
        }
    }
}