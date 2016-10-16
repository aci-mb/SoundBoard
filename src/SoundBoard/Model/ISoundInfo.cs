using System;
using System.ComponentModel;
using SoundBoard.Helpers;

namespace SoundBoard.Model
{
	public interface ISoundInfo : INotifyPropertyChanged, ICloneable
	{
		string FileName { get; set; }
		ErrorInfo ErrorInfo { get; set; }
		string Name { get; set; }
		int VolumeInPercent { get; set; }
		bool IsLooped { get; set; }
		TimeSpan Delay { get; set; }
	}
}