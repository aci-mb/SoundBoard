using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcillatemSoundBoard.Model;

namespace AcillatemSoundBoard.Services
{
    internal class DesignModeSoundBoardRepository : ISoundBoardRepository
    {
        public IEnumerable<SoundBoard> GetSoundBoards()
        {
            yield return new SoundBoard
            {
                Name = "Cthulhu 1",
                Sounds = new ObservableCollection<ISound>(new ISound[]
                {
                    new CsCoreSound
                    {
                        FileName = @"D:\Programme\Steam\SteamApps\common\Machine for Pigs\sounds\ambience\tesla\amb_tesla_basedrone.ogg",
                        IsLooped = true,
                        Delay = TimeSpan.FromSeconds(42.1)
                        
                    },
                    new CsCoreSound
					{
                        FileName = @"D:\Programme\Steam\SteamApps\common\Machine for Pigs\sounds\entities\telephone\telephone_ring_loop.ogg",
                        VolumeInPercent = 50,
                    }
                })
            };

            yield return new SoundBoard
            {
                Name = "DSA 1",
                Sounds = new ObservableCollection<ISound>(new ISound[]
                {
                    new CsCoreSound
					{
                        FileName =
                            @"F:\Musik\Soundtracks\Lord of the Rings\Die Gefährten\13 - The Bridge Of Khazad-Dûm.Mp3",
                    },
                })
            };
        }

        public void SetSoundBoards(IEnumerable<SoundBoard> soundBoards)
        {
            //Nothing to do here, it's just demo data
        }

        public bool AreSoundBoardsDifferent(IEnumerable<SoundBoard> soundBoards)
        {
            return false;
        }
    }
}