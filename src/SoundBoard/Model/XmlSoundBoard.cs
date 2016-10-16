using System.Collections.ObjectModel;
using System.Linq;
using SoundBoard.Services.SoundImplementation;

namespace SoundBoard.Model
{
    public class XmlSoundBoard
    {
        public XmlSoundBoard()
        {
            
        }

        public XmlSoundBoard(SoundBoard originalSoundBoard)
        {
            Name = originalSoundBoard.Name;
            Sounds = (from sound in originalSoundBoard.Sounds
                select new XmlSound(sound)).ToArray();
        }

        public XmlSound[] Sounds { get; set; }

        public string Name { get; set; }

        public SoundBoard ToSoundBoard(ISoundFactory soundFactory)
        {
            return new SoundBoard
            {
                Name = Name,
                Sounds = new ObservableCollection<ISound>(
                    from sound in Sounds
                    select sound.ToSound(soundFactory))
            };
        }
    }
}
