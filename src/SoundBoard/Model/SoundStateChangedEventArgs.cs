using System;

namespace AcillatemSoundBoard.Model
{
	public class SoundStateChangedEventArgs : EventArgs
	{
		public SoundState SoundState { get; set; }

		public SoundStateChangedEventArgs(SoundState soundState)
		{
			SoundState = soundState;
		}
	}
}