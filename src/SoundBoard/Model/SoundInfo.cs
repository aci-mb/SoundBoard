using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AcillatemSoundBoard.Annotations;
using AcillatemSoundBoard.Helpers;
using AcillatemSoundBoard.Properties;

namespace AcillatemSoundBoard.Model
{
	public class SoundInfo : ISoundInfo
	{
		private string _fileName;
		private ErrorInfo _errorInfo;
		private string _name;
		private int _volumeInPercent;
		private bool _isLooped;
		private TimeSpan _delay;

		public SoundInfo()
		{
			_volumeInPercent = 100;
		}

		public virtual string FileName
		{
			get { return _fileName; }
			set
			{
				if (value == _fileName) return;

				UpdateName(_fileName, value);
				_fileName = value;

				VerifyState();

				OnPropertyChanged();
			}
		}

		private void UpdateName(string previousFileName, string newFileName)
		{
			if (string.IsNullOrEmpty(Name) || Name == Path.GetFileNameWithoutExtension(previousFileName))
			{
				Name = Path.GetFileNameWithoutExtension(newFileName);
			}
		}

		public virtual ErrorInfo ErrorInfo
		{
			get { return _errorInfo ?? (_errorInfo = new ErrorInfo()); }
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
				if (value == _isLooped) return;
				_isLooped = value;
				OnPropertyChanged();
			}
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
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void VerifyState()
		{
			if (!File.Exists(FileName))
			{
				ErrorInfo.ErrorMessage = string.Format(Resources.Sound_ErrorInfo_TheFileDoesNotExist, FileName);
				ErrorInfo.HasError = true;
			}
			else
			{
				ErrorInfo.ErrorMessage = string.Empty;
				ErrorInfo.HasError = false;
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}