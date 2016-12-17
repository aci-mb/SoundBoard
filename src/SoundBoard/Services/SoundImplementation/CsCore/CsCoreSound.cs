using System;
using System.IO;
using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using SoundBoard.Model;

namespace SoundBoard.Services.SoundImplementation.CsCore
{
	public class CsCoreSound : SoundInfo, ISound
	{
		private ISoundOut _soundOut;
		private TimeSpan _length;
		private TimeSpan _playbackPosition;
		private readonly object _syncObject = new object();
		private bool _isDisposed;

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

		public TimeSpan PlaybackPosition
		{
			get { return _playbackPosition; }
			set
			{
				if (value.Equals(_playbackPosition)) return;
				_soundOut?.WaveSource.SetPosition(value);
				PrivateSetPlaybackPosition(value);
			}
		}

		/// <summary>
		/// Sets the playback position without setting the position of the <see cref="ISoundOut.WaveSource"/>.
		/// Also raises PropertyChanged events for the properties <see cref="PlaybackPosition"/> and <see cref="PlaybackPositionInSeconds"/>
		/// </summary>
		/// <param name="value"></param>
		private void PrivateSetPlaybackPosition(TimeSpan value)
		{
			_playbackPosition = value;
			OnPropertyChanged(nameof(PlaybackPosition));
			OnPropertyChanged(nameof(PlaybackPositionInSeconds));
		}

		public int PlaybackPositionInSeconds
		{
			get { return (int) PlaybackPosition.TotalSeconds; }
			set
			{
				if (value == (int) PlaybackPosition.TotalSeconds) return;
				
				PlaybackPosition = TimeSpan.FromSeconds(value);
				OnPropertyChanged();
			}
		}

		public TimeSpan Length
		{
			get { return _length; }
			private set
			{
				if (value.Equals(_length)) return;
				_length = value;
				OnPropertyChanged();
			}
		}

		public override string FileName
		{
			get { return base.FileName; }
			set
			{
				base.FileName = value;
				UpdateLength();
			}
		}

		private void UpdateLength()
		{
			if (File.Exists(FileName))
			{
				using (IWaveSource waveSource = CreateWaveSource())
				{
					Length = waveSource.GetLength();
				}
			}
			else
			{
				Length = TimeSpan.Zero;
			}
		}

		public void Play()
		{
			IWaveSource waveSource = CreateWaveSource();
			_soundOut = new WasapiOut();
			_soundOut.Initialize(waveSource);
			_soundOut.Volume = ConvertToImplementationVolume(VolumeInPercent);

			_soundOut.Play();
			_soundOut.Stopped += OnSoundOutStopped;

			new Thread(UpdatePlaybackPositionThread)
			{
				Name = $"{Name} ({FileName}): Playback position update thread"
			}.Start();
		}

		private IWaveSource CreateWaveSource()
		{
			return CodecFactory.Instance.GetCodec(FileName)
				.ToStereo();
		}

		private void OnSoundOutStopped(object sender, PlaybackStoppedEventArgs playbackStoppedEventArgs)
		{
			if (IsLooped)
			{
				_soundOut.Dispose();
				_soundOut = null;
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

			return (float)volumeInPercent / maxPercent;
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
				_soundOut.Dispose();
				_soundOut = null;
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
			_soundOut?.Stop();
			_soundOut?.Dispose();

			_isDisposed = true;
		}

		private void UpdatePlaybackPositionThread()
		{
			try
			{
				while (State == SoundState.Playing && !_isDisposed)
				{
					lock (_syncObject)
					{
						PrivateSetPlaybackPosition(_soundOut.WaveSource.GetPosition());
					}
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
			}
			catch (ThreadAbortException)
			{
			}
		}
	}
}