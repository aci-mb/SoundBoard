using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using SoundBoard.Helpers;
using SoundBoard.Model;
using SoundBoard.Properties;
using SoundBoard.Services;
using SoundBoard.Services.SoundImplementation;
using SoundBoard.Services.SoundImplementation.CsCore;
using SoundBoard.View;

namespace SoundBoard.ViewModel
{
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel, IDropTarget
	{
		private readonly ISoundBoardRepository _soundBoardRepository;
		private readonly IDialogService _dialogService;
		private readonly IKernel _container;
		private ObservableCollection<Model.SoundBoard> _soundBoards;
		private ISound _selectedSound;
		private Model.SoundBoard _selectedSoundBoard;
		private ISound _selectedActiveSound;
		private readonly ISoundFactory _soundFactory;

		public IObservableSoundService SoundService { get; }

		//Don't remove this constructor, it's necessary for blend support!
		public MainWindowViewModel()
			: this(
				CreateSoundBoardRepository(new CsCoreSoundFactory()), new DialogService(),
				new ObservableSoundService(),
				ContainerConfigurator.Kernel,
				new CsCoreSoundFactory())
		{
		}

		public MainWindowViewModel(ISoundBoardRepository soundBoardRepository, IDialogService dialogService,
			IObservableSoundService soundService, IKernel container,
			ISoundFactory soundFactory)
		{
			_soundBoardRepository = soundBoardRepository;
			_dialogService = dialogService;
			_container = container;
			_soundFactory = soundFactory;
			SoundService = soundService;
			Commands = new CommandsRepository(this, _soundFactory);
			SoundContextMenuCommands = new SoundContextMenuCommands(this, _dialogService, container);
			ActiveSoundContextMenuCommands = new ActiveSoundContextMenuCommands(this);
			LoadSoundBoards();
		}

		public ActiveSoundContextMenuCommands ActiveSoundContextMenuCommands { get; private set; }

		public SoundContextMenuCommands SoundContextMenuCommands { get; private set; }

		public ObservableCollection<Model.SoundBoard> SoundBoards
		{
			get { return _soundBoards; }
			set
			{
				if (Equals(value, _soundBoards)) return;
				_soundBoards = value;
				OnPropertyChanged();
			}
		}

		public ISound SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				if (Equals(value, _selectedSound)) return;
				_selectedSound = value;
				OnPropertyChanged();
			}
		}

		public Model.SoundBoard SelectedSoundBoard
		{
			get { return _selectedSoundBoard; }
			set
			{
				if (Equals(value, _selectedSoundBoard)) return;
				_selectedSoundBoard = value;
				OnPropertyChanged();
			}
		}

		public ISound SelectedActiveSound
		{
			get { return _selectedActiveSound; }
			set
			{
				if (Equals(value, _selectedActiveSound)) return;
				_selectedActiveSound = value;
				OnPropertyChanged();
			}
		}

		public CommandsRepository Commands { get; set; }

		private void LoadSoundBoards()
		{
			SoundBoards = new ObservableCollection<Model.SoundBoard>(
				_soundBoardRepository.GetSoundBoards() ??
				Enumerable.Empty<Model.SoundBoard>());

			SelectedSoundBoard = SoundBoards.FirstOrDefault();
		}

		private void SaveSoundBoards()
		{
			_soundBoardRepository.SetSoundBoards(SoundBoards);
		}

		private Dictionary<string, string> Filters => new Dictionary<string, string>
		{
			{
				Resources.MainWindowViewModel_AddSounds_Supported_Files,
				string.Join(Resources.MainWindowViewModel_AddSounds__Supported_Files_Separator,
					_soundFactory.SupportedExtensions)
			}
		};

		private static ISoundBoardRepository CreateSoundBoardRepository(ISoundFactory soundFactory)
		{
			return new XmlSerializingSoundBoardRepository(soundFactory);
		}

		public class CommandsRepository
		{
			private readonly MainWindowViewModel _viewModel;
			private readonly ISoundFactory _soundFactory;

			public CommandsRepository(MainWindowViewModel viewModel, ISoundFactory soundFactory)
			{
				_viewModel = viewModel;
				_soundFactory = soundFactory;
				CreateCommands();
			}

			public ICommand ShutdownAppCommand { get; private set; }

			public ICommand AddSoundCommand { get; private set; }

			public ICommand RemoveSoundCommand { get; private set; }

			public ICommand AddSoundBoardCommand { get; private set; }

			public ICommand RemoveSoundBoardCommand { get; private set; }

			public ICommand ActivateSingleSoundCommand { get; private set; }

			public ICommand DeactivateSingleSoundCommand { get; private set; }

			public ICommand ToggleSoundIsLoopedCommand { get; private set; }

			private void CreateCommands()
			{
				ShutdownAppCommand = new DelegateCommand(o => ShutdownApp());

				AddSoundCommand = new DelegateCommand(o => AddSounds(), o => _viewModel.SelectedSoundBoard != null);

				RemoveSoundCommand = new DelegateCommand(
					o => _viewModel.SelectedSoundBoard.Sounds.Remove(_viewModel.SelectedSound),
					o => _viewModel.SelectedSound != null);

				AddSoundBoardCommand = new DelegateCommand(o => AddSoundBoard());

				RemoveSoundBoardCommand = new DelegateCommand(
					o => _viewModel.SoundBoards.Remove(_viewModel.SelectedSoundBoard),
					o => _viewModel.SelectedSoundBoard != null);

				ActivateSingleSoundCommand = new DelegateCommand(
					ActivateSingleSound,
					o => _viewModel.SelectedSound != null);

				DeactivateSingleSoundCommand = new DelegateCommand(
					DeactivateSingleSound,
					o => _viewModel.SelectedActiveSound != null);

				ToggleSoundIsLoopedCommand = new DelegateCommand(
					ToggleSoundIsLooped);
			}

			private void ShutdownApp()
			{
				_viewModel.SoundService.Clear();
				Settings.Default.Save();
				if (!_viewModel._soundBoardRepository.AreSoundBoardsDifferent(_viewModel.SoundBoards))
				{
					return;
				}

				if (_viewModel._dialogService.QuestionDialog(
					_viewModel._container.Get<MainWindow>(),
					Resources.CommandsRepository_SaveData_Save_soundboards,
					Resources.CommandsRepository_SaveData_Do_you_want_to_save_the_changes_made_to_soundboards_))
				{
					_viewModel.SaveSoundBoards();
				}
			}

			private void DeactivateSingleSound(object parameter)
			{
				if (parameter == null)
				{
					throw new ArgumentNullException(nameof(parameter));
				}
				ISound sound = parameter as ISound;
				if (sound == null)
				{
					throw new ArgumentException(
						string.Format("Parameter must be of type {0}", typeof (ISound)),
						nameof(parameter));
				}

				_viewModel.SoundService.Remove(sound);
			}

			private void ActivateSingleSound(object parameter)
			{
				if (parameter == null)
				{
					throw new ArgumentNullException(nameof(parameter));
				}

				ISound sound = parameter as ISound;
				if (sound == null)
				{
					throw new ArgumentException(
						string.Format("Parameter must be of type {0}", typeof (ISound)),
						nameof(parameter));
				}

				ISound clonedSound = sound.Clone() as ISound;
				_viewModel.SoundService.Add(clonedSound);
				_viewModel.SelectedActiveSound = clonedSound;
			}

			private void ToggleSoundIsLooped(object parameter)
			{
				if (parameter == null)
				{
					throw new ArgumentNullException(nameof(parameter));
				}

				if (!(parameter is ISound))
				{
					throw new ArgumentException(string.Format("Parameter must be of type {0}", typeof (ISound)), nameof(parameter));
				}

				ISound sound = (ISound) parameter;

				sound.IsLooped = !sound.IsLooped;
			}

			private void AddSoundBoard()
			{
				string name = _viewModel._dialogService.NameDialog(
					_viewModel._container.Get<MainWindow>(),
					Resources.CommandsRepository_AddSoundBoard_Add_soundboard,
					Resources.CommandsRepository_AddSoundBoard_Please_enter_a_name_for_the_new_soundboard,
					string.Empty);

				if (!string.IsNullOrEmpty(name))
				{
					Model.SoundBoard addedSoundBoard = new Model.SoundBoard {Name = name};
					_viewModel.SoundBoards.Add(addedSoundBoard);
					_viewModel.SelectedSoundBoard = addedSoundBoard;
				}
			}

			private void AddSounds()
			{
				IEnumerable<string> filesToAdd = _viewModel._dialogService.OpenFileDialog(
					Resources.MainWindowViewModel_AddSounds_Choose_sound_files_to_add,
					_viewModel.Filters);

				foreach (string fileToAdd in filesToAdd)
				{
					ISound sound = _soundFactory.Create();
					sound.FileName = fileToAdd;
					_viewModel.SelectedSoundBoard.Sounds.Add(sound);
					_viewModel.SelectedSound = sound;
				}
			}
		}

		public void DragOver(IDropInfo dropInfo)
		{
			dropInfo.Effects = GetDragDropEffect(dropInfo);
			dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
		}

		private DragDropEffects GetDragDropEffect(IDropInfo dropInfo)
		{
			ISound sound = dropInfo.Data as ISound;
			IEnumerable<ISound> sounds = dropInfo.Data as IEnumerable<ISound>;
			DataObject dataObject = dropInfo.Data as DataObject;
			ICollection<ISound> targetCollection = dropInfo.TargetCollection as ICollection<ISound>;

			if (targetCollection == null)
			{
				return DragDropEffects.None;
			}

			if (sound != null)
			{
				if (targetCollection.Contains(sound))
				{
					return targetCollection.IsReadOnly
						? DragDropEffects.None
						: DragDropEffects.Move;
				}
				else
				{
					return DragDropEffects.Copy;
				}
			}
			if (sounds != null)
			{
				if (sounds.All(targetCollection.Contains))
				{
					return targetCollection.IsReadOnly
						? DragDropEffects.None
						: DragDropEffects.Move;
				}
				else
				{
					return DragDropEffects.Copy;
				}
			}
			if (dataObject != null)
			{
				return DragDropEffects.Copy;
			}

			return DragDropEffects.None;
		}

		public void Drop(IDropInfo dropInfo)
		{
			DragDropEffects dragDropEffects = GetDragDropEffect(dropInfo);
			if (dragDropEffects == DragDropEffects.Copy)
			{
				DropCopy(dropInfo); 
			}
			else if (dragDropEffects == DragDropEffects.Move)
			{
				DropMove(dropInfo);
			}
		}

		private void DropMove(IDropInfo dropInfo)
		{
			ObservableCollection<ISound> targetCollection = dropInfo.TargetCollection as ObservableCollection<ISound>;
			if (targetCollection == null)
			{
				return;
			}

			ISound[] sounds = (dropInfo.Data is ISound
				? new ISound[] {(ISound) dropInfo.Data}
				: dropInfo.Data is IEnumerable<ISound> ? (IEnumerable<ISound>) dropInfo.Data : new ISound[0]).ToArray();

			if (dropInfo.InsertIndex >= targetCollection.IndexOf(sounds.First()) && dropInfo.InsertIndex <= targetCollection.IndexOf(sounds.Last()))
			{
				return;
			}
			bool isUp = targetCollection.IndexOf(sounds[0]) < dropInfo.InsertIndex;
			for (int i = 0; i < sounds.Length; i++)
			{
				int prevDestIndex = i > 0 ? targetCollection.IndexOf(sounds[i - 1]) : dropInfo.InsertIndex - 1;
				targetCollection.Move(targetCollection.IndexOf(sounds[i]), isUp ? prevDestIndex : prevDestIndex + 1);
			}
		}

		private void DropCopy(IDropInfo dropInfo)
		{
			Action<ISound, int> insertSoundAction;
			if (ReferenceEquals(dropInfo.TargetCollection, SoundService.ActiveSounds))
			{
				insertSoundAction = (soundToInsert, i) => { SoundService.Add(soundToInsert); };
			}
			else
			{
				insertSoundAction = (soundToInsert, i) =>
				{
					ObservableCollection<ISound> targetCollection = dropInfo.TargetCollection as ObservableCollection<ISound>;
					targetCollection?.Insert(i, soundToInsert);
				};
			}

			ISound sound = dropInfo.Data as ISound;
			IEnumerable<ISound> sounds = dropInfo.Data as IEnumerable<ISound>;
			DataObject dataObject = dropInfo.Data as DataObject;

			if (sound != null)
			{
				DropSounds(insertSoundAction, dropInfo.InsertIndex, sound);
			}
			else if (sounds != null)
			{
				DropSounds(insertSoundAction, dropInfo.InsertIndex, sounds.ToArray());
			}
			else if (dataObject != null)
			{
				DropFiles(insertSoundAction, dropInfo.InsertIndex, dataObject);
			}
		}

		private void DropSounds(Action<ISound, int> insertSoundAction, int insertIndex, params ISound[] sounds)
		{
			foreach (ISound sound in sounds)
			{
				ISound addedSound = sound.Clone() as ISound;

				insertSoundAction(addedSound, insertIndex++);
			}
		}

		private void DropFiles(Action<ISound, int> insertSoundAction, int insertIndex, DataObject dataObject)
		{
			if (dataObject != null && dataObject.ContainsFileDropList())
			{
				foreach (string fileName in dataObject.GetFileDropList())
				{
					if (_soundFactory.SupportedExtensions.Contains("*" + Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
					{
						ISound sound = _soundFactory.Create();
						sound.FileName = fileName;
						insertSoundAction(sound, insertIndex++);
					}
				}
			}
		}
	}
}