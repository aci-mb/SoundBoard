using System;

namespace SoundBoard.Model
{
    public interface ISound : ISoundInfo, IDisposable
    {
		SoundState State { get; }

		TimeSpan Length { get; }
		TimeSpan PlaybackPosition { get; set; }
		int PlaybackPositionInSeconds { get; set; }

		void Play();
	    void Pause();
	    void Stop();

		event EventHandler<SoundStateChangedEventArgs> SoundStateChanged;
    }
}