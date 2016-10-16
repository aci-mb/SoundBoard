using System.Collections.ObjectModel;
using SoundBoard.Model;

namespace SoundBoard.Services
{
	public interface IObservableSoundService
	{
		ReadOnlyObservableCollection<ISound> ActiveSounds { get; }
		void Add(ISound sound);
		void Remove(ISound sound);
		void Clear();
	}
}