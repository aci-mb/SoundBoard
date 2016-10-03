using System;
using AcillatemSoundBoard.Model;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

namespace AcillatemSoundBoard.Services.SoundImplementation.CsCore
{
	public class CsCoreSound : SoundInfo, ISound
	{
		private ISoundOut _soundOut;

		public CsCoreSound()
		{
			
		}
		public override int VolumeInPercent
		{
			get { return base.VolumeInPercent; }
			set
			{
				base.VolumeInPercent = value;
				AdjustActiveVolume();
			}
		}

		private void AdjustActiveVolume()
		{
			if (State == SoundState.Playing)
			{
				_soundOut.Volume = ConvertToImplementationVolume(VolumeInPercent);
			}
		}

		public void Play()
		{
			IWaveSource waveSource = CodecFactory.Instance.GetCodec(FileName)
				.ToStereo();

			_soundOut = new WasapiOut();
			_soundOut.Initialize(waveSource);
			_soundOut.Volume = ConvertToImplementationVolume(VolumeInPercent);

			_soundOut.Play();
			_soundOut.Stopped += OnSoundOutStopped;
		}

		private void OnSoundOutStopped(object sender, PlaybackStoppedEventArgs playbackStoppedEventArgs)
		{
			if (IsLooped)
			{
				_soundOut.Dispose();
				Play();
			}
			else
			{
				OnSoundStateChanged(new SoundStateChangedEventArgs(SoundState.Stopped));
			}
		}

		private static float ConvertToImplementationVolume(int volumeInPercent)
		{
			const int maxPercent = 100;

			return (float) volumeInPercent/maxPercent;
		}

		public void Pause()
		{
			if (State == SoundState.Playing)
			{
				_soundOut.Pause();
			}
			OnSoundStateChanged(new SoundStateChangedEventArgs(SoundState.Paused));
		}

		public SoundState State
		{
			get
			{
				switch (_soundOut?.PlaybackState)
				{
					case PlaybackState.Stopped:
						return SoundState.Stopped;
					case PlaybackState.Playing:
						return SoundState.Playing;
					case PlaybackState.Paused:
						return SoundState.Paused;

					default:
						return SoundState.Stopped;
				}
			}
		}

		public void Stop()
		{
			if (State != SoundState.Stopped)
			{
				_soundOut.Stopped -= OnSoundOutStopped;
				_soundOut.Stop();
			}
			OnSoundStateChanged(new SoundStateChangedEventArgs(SoundState.Stopped));
		}

		public event EventHandler<SoundStateChangedEventArgs> SoundStateChanged;

		protected virtual void OnSoundStateChanged(SoundStateChangedEventArgs e)
		{
			SoundStateChanged?.Invoke(this, e);
		}

		public void Dispose()
		{
			_soundOut.Dispose();
		}
	}
}