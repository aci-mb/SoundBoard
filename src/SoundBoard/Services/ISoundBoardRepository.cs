using System.Collections.Generic;

namespace SoundBoard.Services
{
    public interface ISoundBoardRepository
    {
        IEnumerable<Model.SoundBoard> GetSoundBoards();
        void SetSoundBoards(IEnumerable<Model.SoundBoard> soundBoards);
        bool AreSoundBoardsDifferent(IEnumerable<Model.SoundBoard> soundBoards);
    }
}