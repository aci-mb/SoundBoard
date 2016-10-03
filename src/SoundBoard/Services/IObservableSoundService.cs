using System.Collections.ObjectModel;
using AcillatemSoundBoard.Model;

namespace AcillatemSoundBoard.Services
{
	public interface IObservableSoundService
	{
		ReadOnlyObservableCollection<ISound> ActiveSounds { get; }
		void Add(ISound sound);
		void Remove(ISound sound);
		void Clear();
	}
}