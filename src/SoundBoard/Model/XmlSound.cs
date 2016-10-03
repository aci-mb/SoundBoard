using System;
using System.Xml.Serialization;
using AcillatemSoundBoard.Services.SoundImplementation;

namespace AcillatemSoundBoard.Model
{
    public sealed class XmlSound
    {
        public XmlSound()
        {
        }

        public XmlSound(ISound originalSound)
        {
            FileName = originalSound.FileName;
            Name = originalSound.Name;
            VolumeInPercent = originalSound.VolumeInPercent;
            IsLooped = originalSound.IsLooped;
            Delay = originalSound.Delay;
        }

        public string FileName { get; set; }

        public string Name { get; set; }

        public int VolumeInPercent { get; set; }

        public bool IsLooped { get; set; }

        public string DelayString
        {
            get { return Delay.ToString(); }
            set { Delay = TimeSpan.Parse(value); }
        }

        [XmlIgnore]
        public TimeSpan Delay { get; set; }

        public ISound ToSound(ISoundFactory soundFactory)
        {
	        ISound sound = soundFactory.Create();
	        sound.Delay = Delay;
	        sound.FileName = FileName;
	        sound.Name = Name;
	        sound.IsLooped = IsLooped;
	        sound.VolumeInPercent = VolumeInPercent;
	        return sound;
        }
    }
}