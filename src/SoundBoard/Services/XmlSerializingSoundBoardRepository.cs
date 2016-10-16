using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SoundBoard.Model;
using SoundBoard.Services.SoundImplementation;

namespace SoundBoard.Services
{
    public class XmlSerializingSoundBoardRepository : ISoundBoardRepository
    {
	    private readonly ISoundFactory _soundFactory;
	    private const string XmlFile = "ActillatemSoundBoardData.xml";

	    public XmlSerializingSoundBoardRepository(ISoundFactory soundFactory)
	    {
		    _soundFactory = soundFactory;
	    }

	    public IEnumerable<Model.SoundBoard> GetSoundBoards()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SoundBoardData));
                using (TextReader reader = new StreamReader(XmlFile))
                {
	                SoundBoardData soundBoardData = serializer.Deserialize(reader) as SoundBoardData;
	                if (soundBoardData == null)
	                {
		                return Enumerable.Empty<Model.SoundBoard>();
	                }

	                return from xmlBoard in soundBoardData.SoundBoards
		                select xmlBoard.ToSoundBoard(_soundFactory);
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<Model.SoundBoard>();
            }
        }

        public void SetSoundBoards(IEnumerable<Model.SoundBoard> soundBoards)
        {
            try
            {
                SoundBoardData dataObject = new SoundBoardData
                {
                    SoundBoards = (from soundBoard in soundBoards
                                  select new XmlSoundBoard(soundBoard)).ToArray()
                };

                XmlSerializer serializer = new XmlSerializer(typeof(SoundBoardData));
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

        public bool AreSoundBoardsDifferent(IEnumerable<Model.SoundBoard> soundBoards)
        {
            IEnumerable<Model.SoundBoard> savedBoards = GetSoundBoards().ToArray();
	        Model.SoundBoard[] soundBoardsArray = soundBoards.ToArray();
	        return ! (savedBoards.Count() == soundBoardsArray.Count()
                && savedBoards.All(savedBoard => soundBoardsArray.Any(currentBoard => AreBoardsEqual(currentBoard, savedBoard))));
        }

        private bool AreBoardsEqual(Model.SoundBoard board, Model.SoundBoard otherBoard)
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
