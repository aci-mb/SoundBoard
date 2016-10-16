using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoundBoard.Model;

namespace SoundBoard.Services
{
	public class ObservableSoundService : IObservableSoundService
	{
		private readonly ObservableCollection<ISound> _activeSounds;

		public ObservableSoundService()
		{
			_activeSounds = new ObservableCollection<ISound>();
			ActiveSounds = new ReadOnlyObservableCollection<ISound>(_activeSounds);
		}

		public ReadOnlyObservableCollection<ISound> ActiveSounds { get; }

		public void Add(ISound sound)
		{
			_activeSounds.Add(sound);
			sound.SoundStateChanged += OnSoundStateChange;
			if (sound.Delay == TimeSpan.Zero)
			{
				sound.Play();
			}
			else
			{
				Task.Run(() => StartSoundDelayed(sound));
			}
		}

		public void Remove(ISound sound)
		{
			sound.Stop();
			sound.Dispose();
			_activeSounds.Remove(sound);
		}

		public void Clear()
		{
			while (_activeSounds.Count > 0)
			{
				Remove(_activeSounds.First());
			}
		}

		private void OnSoundStateChange(object sender, SoundStateChangedEventArgs e)
		{
			ISound sound = sender as ISound;
			if (sound != null)
			{
				if (sound.State == SoundState.Stopped)
				{
					_activeSounds.Remove(sound);
				}
			}
		}

		private void StartSoundDelayed(ISound sound)
		{
			while (sound.Delay > TimeSpan.Zero)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				sound.Delay -= TimeSpan.FromSeconds(1);
			}
			sound.Play();
		}
	}
}