using System.Collections.ObjectModel;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Services;

namespace AcillatemSoundBoard.ViewModel
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<SoundBoard> SoundBoards { get; set; }
        ISound SelectedSound { get; set; }
        SoundBoard SelectedSoundBoard { get; set; }
		IObservableSoundService SoundService { get; }
        ISound SelectedActiveSound { get; set; }
    }
}