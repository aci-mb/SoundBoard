using System;

namespace SoundBoard.Model
{
    public interface ISound : ISoundInfo, IDisposable
    {
		SoundState State { get; }

		void Play();
	    void Pause();
	    void Stop();

		event EventHandler<SoundStateChangedEventArgs> SoundStateChanged;
    }
}