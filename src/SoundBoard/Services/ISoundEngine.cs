using System;
using System.Collections.Generic;
using AcillatemSoundBoard.Model;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

namespace AcillatemSoundBoard.Services
{
    public interface ISoundEngine : IDisposable
    {
        EventHandler<SoundStoppedEventArgs> SoundStopped { get; set; }
        IEnumerable<ISound> Sounds { get; }
        ISound AddSound(string fileName);
        void RemoveSound(ISound sound);
        void ClearSounds();
		string SupportedFilesFilter { get; }
    }

	class CsCoreSoundEngine : ISoundEngine
	{
		private readonly List<ISound> _sounds;

		public CsCoreSoundEngine()
		{
			_sounds = new List<ISound>();
		}

		public void Dispose()
		{
		}

		public EventHandler<SoundStoppedEventArgs> SoundStopped { get; set; }

		public IEnumerable<ISound> Sounds => _sounds;

		public ISound AddSound(string fileName)
		{
			
			return null;
		}

		public void RemoveSound(ISound sound)
		{
		}

		public void ClearSounds()
		{
		}

		public string SupportedFilesFilter => CodecFactory.SupportedFilesFilterEn;
	}

	public class SoundStoppedEventArgs : EventArgs
    {
        public ISound Sound { get; private set; }

        public SoundStoppedEventArgs(ISound sound)
        {
            Sound = sound;
        }
    }
}
