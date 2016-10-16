using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using AcillatemSoundBoard.Annotations;
using AcillatemSoundBoard.Helpers;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Properties;
using AcillatemSoundBoard.Services;
using AcillatemSoundBoard.View;
using Ninject;

namespace AcillatemSoundBoard.ViewModel
{
    public class SoundContextMenuCommands
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;
        private readonly IDialogService _dialogService;
        private readonly IKernel _container;

        public SoundContextMenuCommands(IMainWindowViewModel mainWindowViewModel, [NotNull] IDialogService dialogService, IKernel container)
        {
            if (mainWindowViewModel == null)
            {
                throw new ArgumentNullException("mainWindowViewModel");
            }
            if (dialogService == null)
            {
                throw new ArgumentNullException("dialogService");
            }

            _mainWindowViewModel = mainWindowViewModel;
            _dialogService = dialogService;
            _container = container;
            CreateCommands();
            AddCommandsToCollection();
        }

        private void AddCommandsToCollection()
        {
            Commands = new ObservableCollection<ContextMenuCommand>
            {
                new ContextMenuCommand
                {
                    Command = ActivateSoundsCommand,
                    Name = Resources.SoundContextMenuCommands_AddCommandsToCollection_Start_playback_of_selected_sounds,
                    Icon = new Image()
                    {
                        Width = 24,
                        Height = 24
                    }
                },
                new ContextMenuCommand
                {
                    Command = RenameSoundCommand,
                    Name = Resources.SoundContextMenuCommands_AddCommandsToCollection_Rename_selected_sound,
                    Icon = new Image
                    {
                        Width = 24,
                        Height = 24
                    }
                },
                new ContextMenuCommand
                {
                    Command = RemoveSoundsCommand,
                    Name = Resources.SoundContextMenuCommands_AddCommandsToCollection_Remove_selected_sounds,
                    Icon = new Image()
                    {
                        Width = 24,
                        Height = 24
                    }
                }
            };
        }

        private void RenameSound(object parameter)
        {
            var selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);

            ISound sound = (selectedSounds.Count == 1 ? selectedSounds[0] : null) as ISound;

            if (sound == null)
            {
                throw new ArgumentException("Only a single sound may be selected for renaming", "parameter");
            }

            string newName = _dialogService.NameDialog(
                _container.Get<MainWindow>(),
                Resources.SoundContextMenuCommands_RenameSound_Rename_sound,
                Resources.SoundContextMenuCommands_RenameSound_Please_enter_a_new_name_for_the_sound, 
                sound.Name);

            if (newName != null)
            {
                sound.Name = newName;
            }
        }

        private void CreateCommands()
        {
            ActivateSoundsCommand = new DelegateCommand(
                ActivateSounds,
                AreSoundsSelected);
            
            RemoveSoundsCommand = new DelegateCommand(
                RemoveSounds,
                o => IsSoundBoardSelected()
                    && AreSoundsSelected(o));

            RenameSoundCommand = new DelegateCommand(
                RenameSound,
                IsSingleSoundSelected);
        }

        private bool IsSingleSoundSelected(object parameter)
        {
            try
            {
                var selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);
                return selectedSounds.Count == 1;
            }
            catch
            {
                return false;
            }
        }

        private void RemoveSounds(object parameter)
        {
            var selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);

            while (selectedSounds.Count > 0)
            {
                ISound sound = selectedSounds[0] as ISound;

                _mainWindowViewModel.SelectedSoundBoard.Sounds.Remove(sound);
                selectedSounds.Remove(sound);
            }
        }

        private bool IsSoundBoardSelected()
        {
            return _mainWindowViewModel.SelectedSoundBoard != null;
        }

        private bool AreSoundsSelected(object parameter)
        {
            try
            {
                var selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);
                return selectedSounds.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private void ActivateSounds(object parameter)
        {
            IList selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);

            foreach (var soundObject in selectedSounds)
            {
                _mainWindowViewModel.SoundService.Add((soundObject as ISound).Clone() as ISound);
            }
        }

        private static IList ConvertParameterToSelectedSoundsOrThrow(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            IList selectedSounds = parameter as IList;
            if (selectedSounds == null)
            {
                throw new ArgumentException(
                    string.Format("Parameter must be of type {0}", typeof (IList)),
                    "parameter");
            }
            return selectedSounds;
        }

        public ICommand ActivateSoundsCommand { get; private set; }
        public ICommand RemoveSoundsCommand { get; private set; }
        public ICommand RenameSoundCommand { get; private set; }

        public ObservableCollection<ContextMenuCommand> Commands { get; set; }
    }
}