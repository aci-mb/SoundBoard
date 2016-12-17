using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using SoundBoard.Model;
using SoundBoard.Services;

namespace SoundBoard.ViewModel.Design
{
	public class DesignMainWindowViewModel : IMainWindowViewModel
	{
		public DesignMainWindowViewModel()
		{
			SelectedSoundBoard = new Model.SoundBoard
			{
				Name = "Sample SoundBoard #1",
				Sounds = new ObservableCollection<ISound>
				{
					new DesignModeSound
					{
						Name = "Sample Sound #1",
						Length = TimeSpan.Parse("1:12:23"),
						Delay = TimeSpan.FromSeconds(2),
						IsLooped = true,
						VolumeInPercent = 50
					},
					new DesignModeSound
					{
						Name = "Sample Sound #2",
						Length = TimeSpan.Parse("0:4:4"),
						Delay = TimeSpan.FromSeconds(0),
						IsLooped = true,
						VolumeInPercent = 32
					},
					new DesignModeSound
					{
						Name = "Sample Sound #3",
						Length = TimeSpan.Parse("0:3:12"),
						Delay = TimeSpan.FromSeconds(12),
						IsLooped = false,
						VolumeInPercent = 100
					},

				}
			};
			SoundService = new SampleObservableSoundService();
		}

		class SampleObservableSoundService : IObservableSoundService
		{
			public ReadOnlyObservableCollection<ISound> ActiveSounds =>
				new ReadOnlyObservableCollection<ISound>(new ObservableCollection<ISound>
				{
					new DesignModeSound
					{
						Name = "Sample Sound #1",
						Length = TimeSpan.Parse("1:12:23"),
						Delay = TimeSpan.FromSeconds(2),
						IsLooped = true,
						VolumeInPercent = 50,
						PlaybackPosition = TimeSpan.Parse("0:0:3")
					},
					new DesignModeSound
					{
						Name = "Sample Sound #2",
						Length = TimeSpan.Parse("0:4:4"),
						Delay = TimeSpan.FromSeconds(0),
						IsLooped = true,
						VolumeInPercent = 32,
						PlaybackPosition = TimeSpan.Parse("0:2:32")
					},
					new DesignModeSound
					{
						Name = "Sample Sound #3",
						Length = TimeSpan.Parse("0:3:12"),
						Delay = TimeSpan.FromSeconds(12),
						IsLooped = false,
						VolumeInPercent = 100,
						PlaybackPosition = TimeSpan.Parse("0:01:12")
					},
				});
			public void Add(ISound sound)
			{
			}

			public void Remove(ISound sound)
			{
			}

			public void Clear()
			{
			}
		}

		public ObservableCollection<Model.SoundBoard> SoundBoards { get; set; }

		public ISound SelectedSound { get; set; }

		public Model.SoundBoard SelectedSoundBoard { get; set; }

		public IObservableSoundService SoundService { get; }

		public ISound SelectedActiveSound { get; set; }

		public class DesignModeSound : SoundInfo, ISound
		{
			public void Dispose()
			{
				Trace.WriteLine($"{Name}: Dispose");
			}

			public SoundState State { get; set; }
			public TimeSpan Length { get; set; }
			public TimeSpan PlaybackPosition { get; set; }
			public int PlaybackPositionInSeconds { get; set; }

			public void Play()
			{
				Trace.WriteLine($"{Name}: Play");
			}

			public void Pause()
			{
				Trace.WriteLine($"{Name}: Pause");
			}

			public void Stop()
			{
				Trace.WriteLine($"{Name}: Stop");
			}

			public event EventHandler<SoundStateChangedEventArgs> SoundStateChanged;
		}

		public void SetSoundBoards(IEnumerable<Model.SoundBoard> soundBoards)
		{
			//Nothing to do here, it's just demo data
		}

		public bool AreSoundBoardsDifferent(IEnumerable<Model.SoundBoard> soundBoards)
		{
			return false;
		}
	}
}
