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
		/// All supported extensions in the format of "*.extension".
		/// E. g. "*.mp3"
		/// </summary>
		public string[] SupportedExtensions
			=> new string[]
			{
				"*.3g2", "*.3gp", "*.3gp2", "*.3gpp", "*.aac", "*.adt", "*.adts", "*.aif", "*.aifc", "*.aiff", "*.asf", "*.fla",
				"*.flac", "*.m2ts", "*.m4a", "*.m4v", "*.mov", "*.mp2", "*.mp3", "*.mp4", "*.mp4v", "*.mpeg3", "*.ogg",
				"*.wav", "*.wave", "*.wm", "*.wma", "*.wmv",
			};
	}
}