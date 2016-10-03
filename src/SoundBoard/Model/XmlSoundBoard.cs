using System.Collections.ObjectModel;
using System.Linq;

namespace AcillatemSoundBoard.Model
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

        public SoundBoard ToSoundBoard()
        {
            return new SoundBoard
            {
                Name = Name,
                Sounds = new ObservableCollection<ISound>(
                    from sound in Sounds
                    select sound.ToSound(() => new CsCoreSound()))
            };
        }
    }
}
