using System.Collections.ObjectModel;
using SoundBoard.Model;
using SoundBoard.Services;

namespace SoundBoard.ViewModel
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<Model.SoundBoard> SoundBoards { get; set; }
        ISound SelectedSound { get; set; }
        Model.SoundBoard SelectedSoundBoard { get; set; }
		IObservableSoundService SoundService { get; }
        ISound SelectedActiveSound { get; set; }
    }
}