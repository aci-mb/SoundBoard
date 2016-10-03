using System.Collections.Generic;
using AcillatemSoundBoard.Model;

namespace AcillatemSoundBoard.Services
{
    public interface ISoundBoardRepository
    {
        IEnumerable<SoundBoard> GetSoundBoards();
        void SetSoundBoards(IEnumerable<SoundBoard> soundBoards);
        bool AreSoundBoardsDifferent(IEnumerable<SoundBoard> soundBoards);
    }
}