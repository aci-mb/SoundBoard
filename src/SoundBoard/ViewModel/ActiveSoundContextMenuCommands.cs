using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using AcillatemSoundBoard.Helpers;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Properties;

namespace AcillatemSoundBoard.ViewModel
{
    public class ActiveSoundContextMenuCommands
    {
        private readonly IMainWindowViewModel _mainWindowViewModel;

        public ActiveSoundContextMenuCommands(IMainWindowViewModel mainWindowViewModel)
        {
            if (mainWindowViewModel == null)
            {
                throw new ArgumentNullException("mainWindowViewModel");
            }
            _mainWindowViewModel = mainWindowViewModel;
            CreateCommands();
            AddCommandsToCollection();
        }

        public ObservableCollection<ContextMenuCommand> Commands { get; set; }

        public ICommand RemoveSoundsCommand { get; set; }

        private void AddCommandsToCollection()
        {
            Commands = new ObservableCollection<ContextMenuCommand>
            {
                new ContextMenuCommand
                {
                    Command = RemoveSoundsCommand,
                    Name = Resources.ActiveSoundContextMenuCommands_AddCommandsToCollection_Cancel_playback_of_selected_sounds,
                    Icon = new Image
                    {
                        Height = 24,
                        Width = 24
                    }
                }
            };
        }

        private void CreateCommands()
        {
            RemoveSoundsCommand = new DelegateCommand(
                RemoveSounds,
                AreSoundsSelected);
        }

        private void RemoveSounds(object parameter)
        {
            IList selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);

            while (selectedSounds.Count > 0)
            {
                ISound sound = selectedSounds[0] as ISound;
                
                _mainWindowViewModel.SoundService.Remove(sound);
                selectedSounds.Remove(sound);
            }
        }

        private bool AreSoundsSelected(object parameter)
        {
            try
            {
                IList selectedSounds = ConvertParameterToSelectedSoundsOrThrow(parameter);
                return selectedSounds.Count > 0;
            }
            catch
            {
                return false;
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
                    string.Format("Parameter must be of type {0}", typeof(IList)),
                    "parameter");
            }
            return selectedSounds;
        }
    }
}