using CSCore;
using CSCore.Codecs;
using SoundBoard.Model;

namespace SoundBoard.Services.SoundImplementation.CsCore
{
	public class CsCoreSoundFactory : ISoundFactory
	{
		static CsCoreSoundFactory()
		{
			CodecFactory.Instance.Register("ogg", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
		}

		/// <summary>
		/// Creates a new <see cref="ISound"/> instance for playback
		/// </summary>
		/// <returns></returns>
		public ISound Create()
		{
			return new CsCoreSound();
		}

		/// <summary>
		/// Gets all supported extensions in the format of "*.extension", e. g. "*.mp3"
		/// </summary>
		public string[] SupportedExtensions
			=> new string[]
			{
				"*.mp3", "*.mpeg3", "*.wav", "*.wave", "*.flac", "*.fla", "*.aiff", "*.aif", "*.aifc", "*.aac", "*.adt", "*.adts",
				"*.m2ts", "*.mp2", "*.3g2", "*.3gp2", "*.3gp", "*.3gpp", "*.m4a", "*.m4v", "*.mp4v", "*.mp4", "*.mov", "*.asf",
				"*.wm", "*.wmv", "*.wma", "*.ogg"
			};
	}
}