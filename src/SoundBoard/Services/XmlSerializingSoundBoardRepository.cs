using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Services.SoundImplementation;

namespace AcillatemSoundBoard.Services
{
    public class XmlSerializingSoundBoardRepository : ISoundBoardRepository
    {
	    private readonly ISoundFactory _soundFactory;
	    private const string XmlFile = "ActillatemSoundBoardData.xml";

	    public XmlSerializingSoundBoardRepository(ISoundFactory soundFactory)
	    {
		    _soundFactory = soundFactory;
	    }

	    public IEnumerable<SoundBoard> GetSoundBoards()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AcillatemSoundBoardData));
                using (TextReader reader = new StreamReader(XmlFile))
                {
                    return
                        from xmlBoard
                            in (serializer.Deserialize(reader) as AcillatemSoundBoardData).SoundBoards
                        select xmlBoard.ToSoundBoard(_soundFactory);
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<SoundBoard>();
            }
        }

        public void SetSoundBoards(IEnumerable<SoundBoard> soundBoards)
        {
            try
            {
                AcillatemSoundBoardData dataObject = new AcillatemSoundBoardData
                {
                    SoundBoards = (from soundBoard in soundBoards
                                  select new XmlSoundBoard(soundBoard)).ToArray()
                };

                XmlSerializer serializer = new XmlSerializer(typeof(AcillatemSoundBoardData));
                using (TextWriter textWriter = new StreamWriter(XmlFile))
                {
                    serializer.Serialize(textWriter, dataObject);
                }
            }
            catch
            {
                return;
            }
        }

        public bool AreSoundBoardsDifferent(IEnumerable<SoundBoard> soundBoards)
        {
            IEnumerable<SoundBoard> savedBoards = GetSoundBoards().ToArray();
            return ! (savedBoards.Count() == soundBoards.Count()
                && savedBoards.All(savedBoard => soundBoards.Any(currentBoard => AreBoardsEqual(currentBoard, savedBoard))));
        }

        private bool AreBoardsEqual(SoundBoard board, SoundBoard otherBoard)
        {
            return board.Name == otherBoard.Name
                   && board.Sounds.Count == otherBoard.Sounds.Count
                   && board.Sounds.All(otherSound => otherBoard.Sounds.Any(sound => AreSoundsEqual(sound, otherSound)));
        }

        private bool AreSoundsEqual(ISound sound, ISound otherSound)
        {
            return sound.IsLooped == otherSound.IsLooped
                   && sound.FileName == otherSound.FileName
                   && sound.Name == otherSound.Name
                   && sound.VolumeInPercent == otherSound.VolumeInPercent
                   && sound.Delay == otherSound.Delay;
        }
    }
}
