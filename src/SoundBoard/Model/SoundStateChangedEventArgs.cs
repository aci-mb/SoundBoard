using System;

namespace SoundBoard.Model
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